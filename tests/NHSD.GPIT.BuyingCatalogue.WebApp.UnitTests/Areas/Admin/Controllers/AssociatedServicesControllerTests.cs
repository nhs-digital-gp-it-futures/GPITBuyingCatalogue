using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.PublishStatus;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.AssociatedServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class AssociatedServicesControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AssociatedServicesController).Should()
                .BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "AdminOnly");
            typeof(AssociatedServicesController).Should()
                .BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Admin");
            typeof(AssociatedServicesController).Should()
                .BeDecoratedWith<RouteAttribute>(r => r.Template == "admin/catalogue-solutions/manage/{solutionId}/associated-services");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(AssociatedServicesController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AssociatedServices_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            List<AssociatedService> associatedServices,
            [Frozen] ISolutionsService mockSolutionService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            mockSolutionService.GetSolutionWithServiceAssociations(catalogueItem.Id).Returns(catalogueItem);

            var catalogueItems = associatedServices.Select(a => a.CatalogueItem).ToList();
            mockAssociatedServicesService.GetAllAssociatedServicesForSupplier(catalogueItem.Supplier.Id).Returns(catalogueItems);

            var actual = await controller.AssociatedServices(catalogueItem.Id);

            actual.Should().BeOfType<ViewResult>();

            await mockSolutionService.Received().GetSolutionWithServiceAssociations(catalogueItem.Id);
            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(new AssociatedServicesModel(catalogueItem, catalogueItems), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AssociatedServices_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            AssociatedServicesController controller)
        {
            mockService.GetSolutionWithServiceAssociations(catalogueItemId).Returns(default(CatalogueItem));

            var actual = await controller.AssociatedServices(catalogueItemId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AssociatedServices_InvalidModel_ReturnsViewWithModel(
            Solution solution,
            AssociatedServicesModel model,
            AssociatedServicesController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var actual = (await controller.AssociatedServices(solution.CatalogueItemId, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(model);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AssociatedServices_Saves_And_RedirectsToDesktop(
            CatalogueItemId catalogueItemId,
            AssociatedServicesModel model,
            [Frozen] IAssociatedServicesService mockService,
            AssociatedServicesController controller)
        {
            var actual = await controller.AssociatedServices(catalogueItemId, model);

            await mockService.Received()
                .RelateAssociatedServicesToSolution(
                    catalogueItemId,
                    Arg.Is<IEnumerable<CatalogueItemId>>(
                        l => l.SequenceEqual(
                            model.SelectableAssociatedServices
                                .Where(a => a.Selected)
                                .Select(a => a.CatalogueItemId))));

            actual.Should().BeOfType<RedirectToActionResult>();

            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            actual.As<RedirectToActionResult>().ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            actual.As<RedirectToActionResult>().RouteValues["solutionId"].Should().Be(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddAssociatedService_ValidId_ReturnsViewWithExpectedModel(
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService mockSolutionService,
            AssociatedServicesController controller)
        {
            mockSolutionService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            var actual = await controller.AddAssociatedService(catalogueItem.Id);

            await mockSolutionService.Received().GetSolutionThin(catalogueItem.Id);

            actual.Should().BeOfType<ViewResult>();

            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(new AddAssociatedServiceModel(catalogueItem), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddAssociatedService_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISolutionsService mockService,
            AssociatedServicesController controller)
        {
            mockService.GetSolutionThin(catalogueItemId).Returns(default(CatalogueItem));

            var actual = await controller.AddAssociatedService(catalogueItemId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddAssociatedService_Redirects(
            CatalogueItem catalogueItem,
            CatalogueItemId assocaitedServiceId,
            AddAssociatedServiceModel model,
            [Frozen] ISolutionsService mockSolutionService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            mockSolutionService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            mockAssociatedServicesService.AddAssociatedService(Arg.Any<CatalogueItem>(), Arg.Any<AssociatedServicesDetailsModel>()).Returns(assocaitedServiceId);

            var actual = await controller.AddAssociatedService(catalogueItem.Id, model);

            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AssociatedServicesController.EditAssociatedService));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedService_ValidIds_ReturnsViewWithExpectedModel(
            Solution solution,
            AssociatedService associatedService,
            List<CataloguePrice> listPrices,
            [Frozen] ISolutionsService mockSolutionService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            associatedService.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            associatedService.CatalogueItem.CataloguePrices.AddRange(listPrices);

            mockSolutionService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            mockAssociatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);
            mockAssociatedServicesService.GetAllSolutionsForAssociatedService(associatedService.CatalogueItemId)
                .Returns(Enumerable.Empty<CatalogueItem>().ToList());

            var expectedModel = new EditAssociatedServiceModel(solution.CatalogueItem, associatedService.CatalogueItem);

            var actual = await controller.EditAssociatedService(solution.CatalogueItemId, associatedService.CatalogueItemId);

            await mockSolutionService.Received().GetSolutionThin(solution.CatalogueItemId);
            await mockAssociatedServicesService.Received().GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId);

            actual.Should().BeOfType<ViewResult>();

            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedService_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            [Frozen] ISolutionsService mockSolutionService,
            AssociatedServicesController controller)
        {
            mockSolutionService.GetSolutionThin(solutionId).Returns(default(CatalogueItem));

            var actual = await controller.EditAssociatedService(solutionId, associatedServiceId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedService_InvalidAssociatedServiceId_ReturnsBadRequestResult(
            CatalogueItem solution,
            CatalogueItemId associatedServiceId,
            [Frozen] ISolutionsService mockSolutionService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            mockSolutionService.GetSolutionThin(solution.Id).Returns(solution);

            mockAssociatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId).Returns(default(CatalogueItem));

            var actual = await controller.EditAssociatedService(solution.Id, associatedServiceId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Associated Service found for Id: {associatedServiceId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedServiceDetails_ValidIds_ReturnsViewWithExpectedModel(
            CatalogueItem solution,
            AssociatedService associatedService,
            List<SolutionMergerAndSplitTypesModel> solutionMergerAndSplitTypes,
            [Frozen] ISolutionsService mockSolutionService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            mockSolutionService.GetSolutionThin(solution.Id).Returns(solution);

            var catalogueItem = associatedService.CatalogueItem;
            mockAssociatedServicesService.GetAssociatedServiceWithCataloguePrices(catalogueItem.Id).Returns(catalogueItem);

            mockAssociatedServicesService.GetSolutionsWithMergerAndSplitTypesForButExcludingAssociatedService(catalogueItem.Id).Returns(solutionMergerAndSplitTypes);

            var actual = await controller.EditAssociatedServiceDetails(solution.Id, catalogueItem.Id);

            await mockSolutionService.Received().GetSolutionThin(solution.Id);
            await mockAssociatedServicesService.Received().GetAssociatedServiceWithCataloguePrices(catalogueItem.Id);

            actual.Should().BeOfType<ViewResult>();

            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(new EditAssociatedServiceDetailsModel(solution.SupplierId, solution.Supplier.Name, catalogueItem, solutionMergerAndSplitTypes), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedServiceDetails_InvalidSolutionId_ReturnsBadRequestResult(
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId,
            [Frozen] ISolutionsService mockSolutionService,
            AssociatedServicesController controller)
        {
            mockSolutionService.GetSolutionThin(solutionId).Returns(default(CatalogueItem));

            var actual = await controller.EditAssociatedServiceDetails(solutionId, associatedServiceId);

            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedServicesDetails_InvalidAssociatedServiceId_ReturnsBadRequestResult(
            Solution solution,
            CatalogueItemId associatedServiceId,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAssociatedServicesService mockAssociatedService,
            AssociatedServicesController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);
            mockAssociatedService.GetAssociatedServiceWithCataloguePrices(associatedServiceId).Returns(default(CatalogueItem));

            var actual = await controller.EditAssociatedServiceDetails(solution.CatalogueItemId, associatedServiceId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Associated Service found for Id: {associatedServiceId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditAssociatedServiceDetails_Valid_ReturnsViewWithExpectedRouteValues(
            Solution solution,
            AssociatedService associatedService,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServicesController controller)
        {
            var catalogueItem = associatedService.CatalogueItem;

            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);
            associatedServicesService.GetAssociatedService(catalogueItem.Id).Returns(catalogueItem);

            var model = new EditAssociatedServiceDetailsModel(solution.CatalogueItem.SupplierId, solution.CatalogueItem.Supplier.Name, catalogueItem, null);

            var actual = await controller.EditAssociatedServiceDetails(solution.CatalogueItemId, catalogueItem.Id, model);

            actual.Should().BeOfType<RedirectToActionResult>();

            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AssociatedServicesController.EditAssociatedService));
            actual.As<RedirectToActionResult>().RouteValues["solutionId"].Should().Be(solution.CatalogueItemId);
            actual.As<RedirectToActionResult>().RouteValues["associatedServiceId"].Should().Be(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditAssociatedServiceDetails_InvalidSolutionId_ReturnsBadRequestResult(
            [Frozen] ISolutionsService mockSolutionService,
            AssociatedServicesController controller,
            EditAssociatedServiceDetailsModel model,
            CatalogueItemId solutionId,
            CatalogueItemId associatedServiceId)
        {
            mockSolutionService.GetSolutionThin(solutionId).Returns(default(CatalogueItem));

            var actual = await controller.EditAssociatedServiceDetails(solutionId, associatedServiceId, model);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Solution found for Id: {solutionId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditAssociatedServiceDetails_InvalidAssociatedServiceId_ReturnsBadRequestResult(
            Solution solution,
            CatalogueItemId associatedServiceId,
            EditAssociatedServiceDetailsModel model,
            [Frozen] ISolutionsService solutionsService,
            [Frozen] IAssociatedServicesService mockService,
            AssociatedServicesController controller)
        {
            solutionsService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);
            mockService.GetAssociatedService(associatedServiceId).Returns(default(CatalogueItem));

            var actual = await controller.EditAssociatedServiceDetails(solution.CatalogueItemId, associatedServiceId, model);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Associated Service found for Id: {associatedServiceId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SetPublicationStatus_CallsSavePublicationStatus(
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService mockSolutionService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            [Frozen] IPublicationStatusService mockPublicationStatusService,
            AssociatedServicesController controller)
        {
            catalogueItem.PublishedStatus = PublicationStatus.Draft;

            var model = new EditAssociatedServiceModel { SelectedPublicationStatus = PublicationStatus.Published };

            mockSolutionService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            mockAssociatedServicesService.GetAssociatedService(catalogueItem.Id).Returns(catalogueItem);

            await controller.SetPublicationStatus(catalogueItem.Id, catalogueItem.Id, model);

            await mockPublicationStatusService.Received().SetPublicationStatus(catalogueItem.Id, model.SelectedPublicationStatus);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SetPublicationStatus_ReturnsRedirectToActionResult(
            CatalogueItem catalogueItem,
            [Frozen] ISolutionsService mockSolutionService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            catalogueItem.PublishedStatus = PublicationStatus.Draft;

            var model = new EditAssociatedServiceModel { SelectedPublicationStatus = PublicationStatus.Published };

            mockSolutionService.GetSolutionThin(catalogueItem.Id).Returns(catalogueItem);

            mockAssociatedServicesService.GetAssociatedService(catalogueItem.Id).Returns(catalogueItem);

            var actual = (await controller.SetPublicationStatus(catalogueItem.Id, catalogueItem.Id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(AssociatedServicesController.AssociatedServices));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SetPublicationStatus_InvalidModel_ReturnsViewWithModel(
            Solution solution,
            AssociatedService associatedService,
            [Frozen] ISolutionsService mockSolutionService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var model = new EditAssociatedServiceModel(solution.CatalogueItem, associatedService.CatalogueItem);

            mockSolutionService.GetSolutionThin(solution.CatalogueItemId).Returns(solution.CatalogueItem);

            mockAssociatedServicesService.GetAssociatedService(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var actual = (await controller.SetPublicationStatus(solution.CatalogueItemId, associatedService.CatalogueItemId, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().Be(nameof(AssociatedServicesController.EditAssociatedService));
            actual.Model.Should().BeEquivalentTo(model);
        }
    }
}
