using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.PublishStatus;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class AdditionalServicesControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AdditionalServicesController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_WithInvalidSolutionId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var result = await controller.Index(catalogueItemId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_Index_WithValidSolutionId_ReturnsModel(
            CatalogueItem catalogueItem,
            List<CatalogueItem> additionalServices,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServicesController controller)
        {
            var expectedResult = new AdditionalServicesModel(catalogueItem, additionalServices);

            solutionsService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            additionalServicesService.GetAdditionalServicesBySolutionId(catalogueItem.Id, false).Returns(additionalServices);

            var result = await controller.Index(catalogueItem.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAdditionalService_InvalidSolutionId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] ISolutionsService solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var result = await controller.EditAdditionalService(catalogueItemId, additionalServiceId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAdditionalService_InvalidAdditionalServiceId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            [Frozen] ISolutionsService solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.GetSolutionThin(catalogueItemId).Returns(new CatalogueItem());
            additionalServicesService.GetAdditionalService(catalogueItemId, additionalServiceId).Returns(default(CatalogueItem));

            var result = await controller.EditAdditionalService(catalogueItemId, additionalServiceId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Additional Service with Id {additionalServiceId} found for Solution {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAdditionalService_WithValidIds_ReturnsModel(
            CatalogueItem catalogueItem,
            AdditionalService additionalService,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServicesController controller)
        {
            var expectedResult = new EditAdditionalServiceModel(catalogueItem, additionalService.CatalogueItem);

            solutionsService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            additionalServicesService.GetAdditionalService(catalogueItem.Id, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = await controller.EditAdditionalService(catalogueItem.Id, additionalService.CatalogueItemId);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedResult, opt => opt.Excluding(model => model.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddAdditionalService_InvalidId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockSolutionsService,
            AdditionalServicesController controller)
        {
            mockSolutionsService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var result = await controller.AddAdditionalService(catalogueItemId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddAdditionalService_ValidId_ReturnsModel(
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService solutionsService,
            AdditionalServicesController controller)
        {
            var expectedModel = new EditAdditionalServiceDetailsModel(catalogueItem);

            solutionsService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            var result = await controller.AddAdditionalService(catalogueItem.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(model => model.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddAdditionalService_InvalidModel_ReturnsViewWithModel(
            CatalogueItemId catalogueItemId,
            EditAdditionalServiceDetailsModel model,
            AdditionalServicesController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = await controller.AddAdditionalService(catalogueItemId, model);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddAdditionalService_InvalidId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            EditAdditionalServiceDetailsModel model,
            [Frozen] ISolutionsService solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var result = await controller.AddAdditionalService(catalogueItemId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddAdditionalService_ValidId_RedirectsToEditAdditionalService(
            CatalogueItem catalogueItem,
            EditAdditionalServiceDetailsModel model,
            [Frozen] ISolutionsService solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            var result = await controller.AddAdditionalService(catalogueItem.Id, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServicesController.EditAdditionalService));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAdditionalServiceDetails_InvalidCatalogueItemId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] ISolutionsService solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var result = await controller.EditAdditionalServiceDetails(catalogueItemId, additionalServiceId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAdditionalServiceDetails_InvalidAdditionalServiceId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            [Frozen] ISolutionsService solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.GetSolutionThin(catalogueItemId).Returns(new CatalogueItem());
            additionalServicesService.GetAdditionalService(catalogueItemId, additionalServiceId).Returns(default(CatalogueItem));

            var result = await controller.EditAdditionalServiceDetails(catalogueItemId, additionalServiceId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Additional Service with Id {additionalServiceId} found for Solution {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAdditionalServiceDetails_ValidIds_ReturnsModel(
            AdditionalService additionalService,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServicesController controller)
        {
            var catalogueItem = additionalService.CatalogueItem;
            var expectedModel = new EditAdditionalServiceDetailsModel(catalogueItem, catalogueItem);

            solutionsService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            additionalServicesService.GetAdditionalService(catalogueItem.Id, catalogueItem.Id).Returns(catalogueItem);

            var result = await controller.EditAdditionalServiceDetails(catalogueItem.Id, catalogueItem.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditAdditionalServiceDetails_InvalidModel_ReturnsViewWithModel(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            EditAdditionalServiceDetailsModel model,
            AdditionalServicesController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = await controller.EditAdditionalServiceDetails(catalogueItemId, additionalServiceId, model);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditAdditionalServiceDetails_InvalidCatalogueItemId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            EditAdditionalServiceDetailsModel model,
            [Frozen] ISolutionsService solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var result = await controller.EditAdditionalServiceDetails(catalogueItemId, additionalServiceId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditAdditionalServiceDetails_InvalidAdditionalServiceId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            EditAdditionalServiceDetailsModel model,
            [Frozen] IAdditionalServicesService additionalServicesService,
            [Frozen] ISolutionsService solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.GetSolutionThin(catalogueItemId).Returns(new CatalogueItem());
            additionalServicesService.GetAdditionalService(catalogueItemId, additionalServiceId).Returns(default(CatalogueItem));

            var result = await controller.EditAdditionalServiceDetails(catalogueItemId, additionalServiceId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Additional Service with Id {additionalServiceId} found for Solution {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditAdditionalServiceDetails_ValidModel_RedirectsToEditAdditionalService(
            CatalogueItem catalogueItem,
            AdditionalService additionalService,
            EditAdditionalServiceDetailsModel model,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServicesController controller)
        {
            solutionsService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            additionalServicesService.GetAdditionalService(catalogueItem.Id, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = await controller.EditAdditionalServiceDetails(catalogueItem.Id, additionalService.CatalogueItemId, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServicesController.EditAdditionalService));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SetPublicationStatus_CallsSavePublicationStatus(
            CatalogueItem catalogueItem,
            AdditionalService additionalService,
            [Frozen] ISolutionsService mockSolutionService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            [Frozen] IPublicationStatusService publicationStatusService,
            AdditionalServicesController controller)
        {
            additionalService.CatalogueItem.PublishedStatus = PublicationStatus.Draft;

            var model = new EditAdditionalServiceModel { SelectedPublicationStatus = PublicationStatus.Published };

            mockSolutionService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            additionalServicesService.GetAdditionalService(catalogueItem.Id, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            await controller.SetPublicationStatus(catalogueItem.Id, additionalService.CatalogueItemId, model);

            await publicationStatusService.Received().SetPublicationStatus(additionalService.CatalogueItemId, model.SelectedPublicationStatus);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SetPublicationStatus_ReturnsRedirectToActionResult(
            CatalogueItem catalogueItem,
            AdditionalService additionalService,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServicesController controller)
        {
            additionalService.CatalogueItem.PublishedStatus = PublicationStatus.Draft;

            var model = new EditAdditionalServiceModel { SelectedPublicationStatus = PublicationStatus.Published };

            solutionsService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            additionalServicesService.GetAdditionalService(catalogueItem.Id, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var actual = (await controller.SetPublicationStatus(catalogueItem.Id, additionalService.CatalogueItemId, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(AdditionalServicesController.Index));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SetPublicationStatus_InvalidModel_ReturnsViewWithModel(
            CatalogueItem catalogueItem,
            AdditionalService additionalService,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServicesController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var model = new EditAdditionalServiceModel(catalogueItem, additionalService.CatalogueItem);

            solutionsService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            additionalServicesService.GetAdditionalService(catalogueItem.Id, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var actual = (await controller.SetPublicationStatus(catalogueItem.Id, additionalService.CatalogueItemId, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().Be(nameof(AdditionalServicesController.EditAdditionalService));
            actual.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditCapabilities_InvalidId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] IAdditionalServicesService additionalServicesService,
            [Frozen] ISolutionsService solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.GetSolutionThin(catalogueItemId).Returns(new CatalogueItem());
            additionalServicesService.GetAdditionalService(catalogueItemId, additionalServiceId).Returns(default(CatalogueItem));

            var result = await controller.EditCapabilities(catalogueItemId, additionalServiceId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Additional Service with Id {additionalServiceId} found for Solution {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditCapabilities_ValidId_ReturnsModel(
            Solution solution,
            AdditionalService additionalService,
            IReadOnlyList<CapabilityCategory> capabilityCategories,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAdditionalServicesService additionalServicesService,
            [Frozen] ICapabilitiesService capabilitiesService,
            AdditionalServicesController controller)
        {
            var expectedModel = new EditCapabilitiesModel(additionalService.CatalogueItem, capabilityCategories)
            {
                SolutionName = solution.CatalogueItem.Name,
            };

            capabilitiesService.GetCapabilitiesByCategory().Returns(capabilityCategories.ToList());

            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            var result = await controller.EditCapabilities(solution.CatalogueItemId, additionalService.CatalogueItemId);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditCapabilities_InvalidModel_ReturnsViewWithModel(
            Solution solution,
            AdditionalService additionalService,
            EditCapabilitiesModel model,
            AdditionalServicesController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var result = await controller.EditCapabilities(solution.CatalogueItemId, additionalService.CatalogueItemId, model);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(model, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditCapabilities_InvalidId_ReturnsBadRequestObjectResult(
            Solution solution,
            AdditionalService additionalService,
            EditCapabilitiesModel model,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServicesController controller)
        {
            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(default(CatalogueItem));

            var result = await controller.EditCapabilities(solution.CatalogueItemId, additionalService.CatalogueItemId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Additional Service with Id {additionalService.CatalogueItemId} found for Solution {solution.CatalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditCapabilities_ValidModel_AddsCapabilitiesToCatalogueItem(
            Solution solution,
            AdditionalService additionalService,
            EditCapabilitiesModel model,
            [Frozen] IAdditionalServicesService additionalServicesService,
            [Frozen] ICapabilitiesService capabilitiesService,
            AdditionalServicesController controller)
        {
            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            _ = await controller.EditCapabilities(solution.CatalogueItemId, additionalService.CatalogueItemId, model);

            await capabilitiesService.Received().AddCapabilitiesToCatalogueItem(additionalService.CatalogueItemId, Arg.Any<SaveCatalogueItemCapabilitiesModel>());
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditCapabilities_ValidModel_RedirectsToEditAdditionalService(
            Solution solution,
            AdditionalService additionalService,
            EditCapabilitiesModel model,
            [Frozen] IAdditionalServicesService additionalServicesService,
            AdditionalServicesController controller)
        {
            additionalServicesService.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId).Returns(additionalService.CatalogueItem);

            var result = await controller.EditCapabilities(solution.CatalogueItemId, additionalService.CatalogueItemId, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServicesController.EditAdditionalService));
            result.As<RedirectToActionResult>().RouteValues.Should().Contain(
                new KeyValuePair<string, object>("solutionId", solution.CatalogueItemId),
                new KeyValuePair<string, object>("additionalServiceId", additionalService.CatalogueItemId));
        }
    }
}
