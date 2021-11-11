using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Capabilities;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AdditionalServices;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.CapabilityModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class AdditionalServicesControllerTests
    {
        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AdditionalServicesController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_WithInvalidSolutionId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.Index(catalogueItemId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_WithValidSolutionId_ReturnsModel(
            CatalogueItem catalogueItem,
            List<CatalogueItem> additionalServices,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            var expectedResult = new AdditionalServicesModel(catalogueItem, additionalServices);

            solutionsService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            additionalServicesService.Setup(s => s.GetAdditionalServicesBySolutionId(catalogueItem.Id))
                .ReturnsAsync(additionalServices);

            var result = await controller.Index(catalogueItem.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedResult);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditAdditionalService_InvalidSolutionId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] Mock<ISolutionsService> solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.EditAdditionalService(catalogueItemId, additionalServiceId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditAdditionalService_InvalidAdditionalServiceId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            additionalServicesService.Setup(s => s.GetAdditionalService(catalogueItemId, additionalServiceId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.EditAdditionalService(catalogueItemId, additionalServiceId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Additional Service with Id {additionalServiceId} found for Solution {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditAdditionalService_WithValidIds_ReturnsModel(
            CatalogueItem catalogueItem,
            AdditionalService additionalService,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            var expectedResult = new EditAdditionalServiceModel(catalogueItem, additionalService.CatalogueItem);

            solutionsService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            additionalServicesService.Setup(s => s.GetAdditionalService(catalogueItem.Id, additionalService.CatalogueItemId))
                .ReturnsAsync(additionalService.CatalogueItem);

            var result = await controller.EditAdditionalService(catalogueItem.Id, additionalService.CatalogueItemId);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedResult, opt => opt.Excluding(model => model.BackLink));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddAdditionalService_InvalidId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AdditionalServicesController controller)
        {
            mockSolutionsService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.AddAdditionalService(catalogueItemId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddAdditionalService_ValidId_ReturnsModel(
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> solutionsService,
            AdditionalServicesController controller)
        {
            var expectedModel = new EditAdditionalServiceDetailsModel(catalogueItem);

            solutionsService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var result = await controller.AddAdditionalService(catalogueItem.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(model => model.BackLink));
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_AddAdditionalService_InvalidId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            EditAdditionalServiceDetailsModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.AddAdditionalService(catalogueItemId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddAdditionalService_ValidId_RedirectsToEditAdditionalService(
            CatalogueItem catalogueItem,
            EditAdditionalServiceDetailsModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var result = await controller.AddAdditionalService(catalogueItem.Id, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServicesController.EditAdditionalService));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditAdditionalServiceDetails_InvalidCatalogueItemId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] Mock<ISolutionsService> solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.EditAdditionalServiceDetails(catalogueItemId, additionalServiceId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditAdditionalServiceDetails_InvalidAdditionalServiceId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            additionalServicesService.Setup(s => s.GetAdditionalService(catalogueItemId, additionalServiceId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.EditAdditionalServiceDetails(catalogueItemId, additionalServiceId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Additional Service with Id {additionalServiceId} found for Solution {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditAdditionalServiceDetails_ValidIds_ReturnsModel(
            AdditionalService additionalService,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            var catalogueItem = additionalService.CatalogueItem;
            var expectedModel = new EditAdditionalServiceDetailsModel(catalogueItem, catalogueItem);

            solutionsService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            additionalServicesService.Setup(s => s.GetAdditionalService(catalogueItem.Id, catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var result = await controller.EditAdditionalServiceDetails(catalogueItem.Id, catalogueItem.Id);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_EditAdditionalServiceDetails_InvalidCatalogueItemId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            EditAdditionalServiceDetailsModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            AdditionalServicesController controller)
        {
            solutionsService.Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.EditAdditionalServiceDetails(catalogueItemId, additionalServiceId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditAdditionalServiceDetails_InvalidAdditionalServiceId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            EditAdditionalServiceDetailsModel model,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            additionalServicesService.Setup(s => s.GetAdditionalService(catalogueItemId, additionalServiceId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.EditAdditionalServiceDetails(catalogueItemId, additionalServiceId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Additional Service with Id {additionalServiceId} found for Solution {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditAdditionalServiceDetails_ValidModel_RedirectsToEditAdditionalService(
            CatalogueItem catalogueItem,
            AdditionalService additionalService,
            EditAdditionalServiceDetailsModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            solutionsService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            additionalServicesService.Setup(s => s.GetAdditionalService(catalogueItem.Id, additionalService.CatalogueItemId))
                .ReturnsAsync(additionalService.CatalogueItem);

            var result = await controller.EditAdditionalServiceDetails(catalogueItem.Id, additionalService.CatalogueItemId, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServicesController.EditAdditionalService));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_SamePublicationStatus_DoesNotCallSavePublicationStatus(
            CatalogueItem catalogueItem,
            AdditionalService additionalService,
            EditAdditionalServiceModel model,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            model.SelectedPublicationStatus = catalogueItem.PublishedStatus;

            solutionsService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            additionalServicesService.Setup(s => s.GetAdditionalService(catalogueItem.Id, additionalService.CatalogueItemId))
                .ReturnsAsync(additionalService.CatalogueItem);

            await controller.SetPublicationStatus(catalogueItem.Id, additionalService.CatalogueItemId, model);

            solutionsService.Verify(s => s.SavePublicationStatus(catalogueItem.Id, model.SelectedPublicationStatus), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_CallsSavePublicationStatus(
            CatalogueItem catalogueItem,
            AdditionalService additionalService,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            additionalService.CatalogueItem.PublishedStatus = PublicationStatus.Draft;

            var model = new EditAdditionalServiceModel { SelectedPublicationStatus = PublicationStatus.Published };

            mockSolutionService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            additionalServicesService.Setup(s => s.GetAdditionalService(catalogueItem.Id, additionalService.CatalogueItemId))
                .ReturnsAsync(additionalService.CatalogueItem);

            await controller.SetPublicationStatus(catalogueItem.Id, additionalService.CatalogueItemId, model);

            additionalServicesService.Verify(s => s.SavePublicationStatus(catalogueItem.Id, additionalService.CatalogueItemId, model.SelectedPublicationStatus));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_ReturnsRedirectToActionResult(
            CatalogueItem catalogueItem,
            AdditionalService additionalService,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            additionalService.CatalogueItem.PublishedStatus = PublicationStatus.Draft;

            var model = new EditAdditionalServiceModel { SelectedPublicationStatus = PublicationStatus.Published };

            solutionsService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            additionalServicesService.Setup(s => s.GetAdditionalService(catalogueItem.Id, additionalService.CatalogueItemId))
                .ReturnsAsync(additionalService.CatalogueItem);

            var actual = (await controller.SetPublicationStatus(catalogueItem.Id, additionalService.CatalogueItemId, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(AdditionalServicesController.Index));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SetPublicationStatus_InvalidModel_ReturnsViewWithModel(
            CatalogueItem catalogueItem,
            AdditionalService additionalService,
            [Frozen] Mock<ISolutionsService> solutionsService,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var model = new EditAdditionalServiceModel(catalogueItem, additionalService.CatalogueItem);

            solutionsService.Setup(s => s.GetSolution(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            additionalServicesService.Setup(s => s.GetAdditionalService(catalogueItem.Id, additionalService.CatalogueItemId))
                .ReturnsAsync(additionalService.CatalogueItem);

            var actual = (await controller.SetPublicationStatus(catalogueItem.Id, additionalService.CatalogueItemId, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().Be(nameof(AdditionalServicesController.EditAdditionalService));
            actual.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditCapabilities_InvalidId_ReturnsBadRequestObjectResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            additionalServicesService.Setup(s => s.GetAdditionalService(catalogueItemId, additionalServiceId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.EditCapabilities(catalogueItemId, additionalServiceId);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Additional Service with Id {additionalServiceId} found for Solution {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditCapabilities_ValidId_ReturnsModel(
            Solution solution,
            AdditionalService additionalService,
            IReadOnlyList<CapabilityCategory> capabilityCategories,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            AdditionalServicesController controller)
        {
            var expectedModel = new EditCapabilitiesModel(additionalService.CatalogueItem, capabilityCategories);

            capabilitiesService.Setup(s => s.GetCapabilitiesByCategory())
                .ReturnsAsync(capabilityCategories.ToList());

            additionalServicesService.Setup(s => s.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId))
                .ReturnsAsync(additionalService.CatalogueItem);

            var result = await controller.EditCapabilities(solution.CatalogueItemId, additionalService.CatalogueItemId);

            result.As<ViewResult>().Should().NotBeNull();
            result.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [CommonAutoData]
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
        [CommonAutoData]
        public static async Task Post_EditCapabilities_InvalidId_ReturnsBadRequestObjectResult(
            Solution solution,
            AdditionalService additionalService,
            EditCapabilitiesModel model,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            additionalServicesService.Setup(s => s.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var result = await controller.EditCapabilities(solution.CatalogueItemId, additionalService.CatalogueItemId, model);

            result.As<BadRequestObjectResult>().Should().NotBeNull();
            result.As<BadRequestObjectResult>().Value.Should().Be($"No Additional Service with Id {additionalService.CatalogueItemId} found for Solution {solution.CatalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditCapabilities_ValidModel_AddsCapabilitiesToCatalogueItem(
            Solution solution,
            AdditionalService additionalService,
            EditCapabilitiesModel model,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            [Frozen] Mock<ICapabilitiesService> capabilitiesService,
            AdditionalServicesController controller)
        {
            additionalServicesService.Setup(s => s.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId))
                .ReturnsAsync(additionalService.CatalogueItem);

            _ = await controller.EditCapabilities(solution.CatalogueItemId, additionalService.CatalogueItemId, model);

            capabilitiesService.Verify(s => s.AddCapabilitiesToCatalogueItem(additionalService.CatalogueItemId, It.IsAny<SaveCatalogueItemCapabilitiesModel>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditCapabilities_ValidModel_RedirectsToEditAdditionalService(
            Solution solution,
            AdditionalService additionalService,
            EditCapabilitiesModel model,
            [Frozen] Mock<IAdditionalServicesService> additionalServicesService,
            AdditionalServicesController controller)
        {
            additionalServicesService.Setup(s => s.GetAdditionalService(solution.CatalogueItemId, additionalService.CatalogueItemId))
                .ReturnsAsync(additionalService.CatalogueItem);

            var result = await controller.EditCapabilities(solution.CatalogueItemId, additionalService.CatalogueItemId, model);

            result.As<RedirectToActionResult>().Should().NotBeNull();
            result.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServicesController.EditAdditionalService));
            result.As<RedirectToActionResult>().RouteValues.Should().Contain(
                new KeyValuePair<string, object>("solutionId", solution.CatalogueItemId),
                new KeyValuePair<string, object>("additionalServiceId", additionalService.CatalogueItemId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ManageListPrices_ValidId_ReturnsCataloguePrice(
        CatalogueItemId catalogueItemId,
        CatalogueItemId additionalServiceId,
        CatalogueItem catalogueItem,
        CatalogueItem additionalService,
        [Frozen] Mock<ISolutionsService> mockSolutionsService,
        [Frozen] Mock<IAdditionalServicesService> mockAdditionalServicesService,
        AdditionalServicesController controller)
        {
            mockSolutionsService
                .Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockAdditionalServicesService
                .Setup(s => s.GetAdditionalService(catalogueItemId, additionalServiceId))
                .ReturnsAsync(additionalService);

            var actual = (await controller.ManageListPrices(catalogueItemId, additionalServiceId)).As<ViewResult>();

            actual.ViewName.Should().BeNull();

            var manageListPricesModel = actual.Model.As<ManageListPricesModel>();
            manageListPricesModel.CataloguePrices.Should().BeEquivalentTo(additionalService.CataloguePrices);
            manageListPricesModel.CatalogueItemId.Should().Be(additionalService.Id);
            manageListPricesModel.CatalogueName.Should().Be(additionalService.Name);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ManageListPrices_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] Mock<IAdditionalServicesService> mockAdditionalServicesService,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            AdditionalServicesController controller,
            CatalogueItem catalogueItem)
        {
            mockSolutionService
                .Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockAdditionalServicesService
                .Setup(s => s.GetAdditionalService(catalogueItemId, additionalServiceId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.ManageListPrices(catalogueItemId, additionalServiceId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Additional Service found for Id: {additionalServiceId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddListPrice_ValidId_ReturnsModel(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] Mock<IAdditionalServicesService> mockAdditionalServicesService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AdditionalServicesController controller,
            CatalogueItem catalogueItem,
            CatalogueItem additionalService)
        {
            mockSolutionsService
                .Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockAdditionalServicesService
                .Setup(s => s.GetAdditionalService(catalogueItemId, additionalServiceId))
                .ReturnsAsync(additionalService);

            var actual = await controller.AddListPrice(catalogueItemId, additionalServiceId);

            mockSolutionsService.Verify(s => s.GetSolution(catalogueItemId));
            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();

            var model = actual.As<ViewResult>().Model.As<EditListPriceModel>();
            model.ItemId.Should().Be(additionalService.Id);
            model.ItemName.Should().Be(additionalService.Name);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddListPrice_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AdditionalServicesController controller)
        {
            mockSolutionsService
                .Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await controller.AddListPrice(catalogueItemId, additionalServiceId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddListPrice_InvalidAdditionalServiceId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] Mock<IAdditionalServicesService> mockAdditionalServicesService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AdditionalServicesController controller,
            CatalogueItem catalogueItem)
        {
            mockSolutionsService
                .Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockAdditionalServicesService
                .Setup(s => s.GetAdditionalService(catalogueItemId, additionalServiceId))
                .ReturnsAsync((CatalogueItem)null);

            var actual = (await controller.AddListPrice(catalogueItemId, additionalServiceId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Additional Service found for Id: {additionalServiceId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddListPrice_ModelStateValid_RedirectsToManageListPrices(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] Mock<IAdditionalServicesService> mockAdditionalServicesService,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            AdditionalServicesController controller,
            CatalogueItem catalogueItem)
        {
            const decimal price = 3.21M;
            var solutionId = additionalServiceId;
            var editListPriceModel = new EditListPriceModel(catalogueItem)
            {
                Price = price,
                SelectedProvisioningType = ProvisioningType.Patient,
                Unit = "per patient",
            };

            mockSolutionsService
                .Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            mockAdditionalServicesService
                .Setup(s => s.GetAdditionalService(catalogueItemId, additionalServiceId))
                .ReturnsAsync(catalogueItem);

            var actual = await controller.AddListPrice(solutionId, additionalServiceId, editListPriceModel);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ControllerName.Should().BeNull();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServicesController.ManageListPrices));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditListPrice_ValidId_ReturnsModel(
            CatalogueItemId catalogueItemId,
            CatalogueItemId additionalServiceId,
            [Frozen] Mock<IListPricesService> mockListPricesService,
            AdditionalServicesController controller,
            CatalogueItem catalogueItem)
        {
            var cataloguePriceId = catalogueItem
                .CataloguePrices
                .First()
                .CataloguePriceId;

            mockListPricesService
                .Setup(s => s.GetCatalogueItemWithPrices(additionalServiceId))
                .ReturnsAsync(catalogueItem);

            var actual = await controller.EditListPrice(catalogueItemId, additionalServiceId, cataloguePriceId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();
            var model = actual.As<ViewResult>().Model;
            model.Should().BeOfType<EditListPriceModel>();

            model.As<EditListPriceModel>().ItemName.Should().Be(catalogueItem.Name);
            model.As<EditListPriceModel>().CataloguePriceId.Should().Be(cataloguePriceId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditListPrice_InvalidListPriceId_RedirectsToManageListPrices(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<IListPricesService> mockListPricesService,
            AdditionalServicesController controller)
        {
            catalogueItem.CataloguePrices.Clear();

            const int cataloguePriceId = int.MaxValue;

            mockListPricesService
                .Setup(s => s.GetCatalogueItemWithPrices(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = await controller.EditListPrice(catalogueItemId, catalogueItemId, cataloguePriceId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ControllerName.Should().BeNull();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServicesController.ManageListPrices));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditListPrice_ModelStateValid_RedirectsToManageListPrices(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<IListPricesService> mockListPricesService,
            AdditionalServicesController controller)
        {
            const decimal price = 3.21M;
            var cataloguePriceId = catalogueItem
                .CataloguePrices
                .First()
                .CataloguePriceId;

            var editListPriceModel = new EditListPriceModel(catalogueItem)
            {
                CataloguePriceId = cataloguePriceId,
                Price = price,
                SelectedProvisioningType = ProvisioningType.Patient,
                Unit = "per patient",
            };

            var actual = await controller.EditListPrice(catalogueItemId, catalogueItem.Id, cataloguePriceId, editListPriceModel);

            mockListPricesService.Verify(s => s.UpdateListPrice(catalogueItem.Id, It.IsAny<ServiceContracts.Models.SaveListPriceModel>()));

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ControllerName.Should().BeNull();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AdditionalServicesController.ManageListPrices));
        }
    }
}
