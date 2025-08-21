using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.CatalogueItems;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AssociatedServices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.PublishStatus;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Suppliers;
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
            [Frozen] ICatalogueItemService mockCatalogueItemService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            mockCatalogueItemService.GetCatalogueItemWithSupplierServiceAssociations(catalogueItem.Id).Returns(catalogueItem);

            var catalogueItems = associatedServices.Select(a => a.CatalogueItem).ToList();
            mockAssociatedServicesService.GetAllAssociatedServicesForSupplier(catalogueItem.Supplier.Id).Returns(catalogueItems);

            var actual = await controller.AssociatedServices(catalogueItem.Id);

            actual.Should().BeOfType<ViewResult>();

            await mockCatalogueItemService.Received().GetCatalogueItemWithSupplierServiceAssociations(catalogueItem.Id);
            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(new AssociatedServicesModel(catalogueItem, catalogueItems), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AssociatedServices_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ICatalogueItemService mockCatalogueItemService,
            AssociatedServicesController controller)
        {
            mockCatalogueItemService.GetCatalogueItemWithSupplierServiceAssociations(catalogueItemId).Returns(default(CatalogueItem));

            var actual = await controller.AssociatedServices(catalogueItemId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Catalogue Item found for Id: {catalogueItemId}");
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

            var result = actual.Should().BeOfType<RedirectToActionResult>().Subject;

            result.ActionName.Should().Be(nameof(CatalogueSolutionsController.ManageCatalogueSolution));
            result.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
            result.RouteValues.Should().ContainKey("solutionId").WhoseValue.Should().Be(catalogueItemId);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddAssociatedService_ValidId_ReturnsViewWithExpectedModel(
            Supplier supplier,
            CatalogueItemId catalogueItemId,
            [Frozen] ISuppliersService supplierService,
            AssociatedServicesController controller)
        {
            supplierService.GetSupplier(catalogueItemId.SupplierId).Returns(supplier);

            var actual = await controller.AddAssociatedService(catalogueItemId);

            await supplierService.Received().GetSupplier(catalogueItemId.SupplierId);

            actual.Should().BeOfType<ViewResult>();

            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(new AddAssociatedServiceModel(supplier), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddAssociatedService_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] ISuppliersService supplierService,
            AssociatedServicesController controller)
        {
            supplierService.GetSupplier(catalogueItemId.SupplierId).Returns((Supplier)null);

            var actual = await controller.AddAssociatedService(catalogueItemId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Supplier found for Id: {catalogueItemId.SupplierId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddAssociatedService_Redirects(
            CatalogueItemId catalogueItemId,
            CatalogueItemId associatedServiceId,
            AddAssociatedServiceModel model,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            mockAssociatedServicesService.AddAssociatedService(Arg.Any<CatalogueItemId>(), Arg.Any<AssociatedServicesDetailsModel>()).Returns(associatedServiceId);

            var actual = await controller.AddAssociatedService(catalogueItemId, model);

            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(AssociatedServicesController.EditAssociatedService));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedService_ValidIds_ReturnsViewWithExpectedModel(
            Supplier supplier,
            CatalogueItemId catalogueItemId,
            AssociatedService associatedService,
            List<CataloguePrice> listPrices,
            [Frozen] ISuppliersService supplierService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            associatedService.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            associatedService.CatalogueItem.CataloguePrices.AddRange(listPrices);

            supplierService.GetSupplier(catalogueItemId.SupplierId).Returns(supplier);

            mockAssociatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);
            mockAssociatedServicesService.GetAllSolutionsForAssociatedService(associatedService.CatalogueItemId)
                .Returns(Enumerable.Empty<CatalogueItem>().ToList());

            var expectedModel = new EditAssociatedServiceModel(supplier, catalogueItemId, associatedService.CatalogueItem);

            var actual = await controller.EditAssociatedService(catalogueItemId, associatedService.CatalogueItemId);

            await supplierService.Received().GetSupplier(catalogueItemId.SupplierId);
            await mockAssociatedServicesService.Received().GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId);

            actual.Should().BeOfType<ViewResult>();

            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedService_InvalidSupplierId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId associatedServiceId,
            [Frozen] ISuppliersService supplierService,
            AssociatedServicesController controller)
        {
            supplierService.GetSupplier(catalogueItemId.SupplierId).Returns((Supplier)null);

            var actual = await controller.EditAssociatedService(catalogueItemId, associatedServiceId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Supplier found for Id: {catalogueItemId.SupplierId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedService_InvalidAssociatedServiceId_ReturnsBadRequestResult(
            Supplier supplier,
            CatalogueItemId catalogueItemId,
            CatalogueItemId associatedServiceId,
            [Frozen] ISuppliersService supplierService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            supplierService.GetSupplier(catalogueItemId.SupplierId).Returns(supplier);

            mockAssociatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId).Returns(default(CatalogueItem));

            var actual = await controller.EditAssociatedService(catalogueItemId, associatedServiceId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Associated Service found for Id: {associatedServiceId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedServiceDetails_ValidIds_ReturnsViewWithExpectedModel(
            Supplier supplier,
            CatalogueItemId catalogueItemId,
            AssociatedService associatedService,
            List<SolutionMergerAndSplitTypesModel> solutionMergerAndSplitTypes,
            [Frozen] ISuppliersService suppliersService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            suppliersService.GetSupplier(catalogueItemId.SupplierId).Returns(supplier);

            var catalogueItem = associatedService.CatalogueItem;
            mockAssociatedServicesService.GetAssociatedServiceWithCataloguePrices(catalogueItem.Id).Returns(catalogueItem);

            mockAssociatedServicesService.GetSolutionsWithMergerAndSplitTypesForButExcludingAssociatedService(catalogueItem.Id).Returns(solutionMergerAndSplitTypes);

            var actual = await controller.EditAssociatedServiceDetails(catalogueItemId, catalogueItem.Id);

            await suppliersService.Received().GetSupplier(catalogueItemId.SupplierId);
            await mockAssociatedServicesService.Received().GetAssociatedServiceWithCataloguePrices(catalogueItem.Id);

            actual.Should().BeOfType<ViewResult>();

            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(new EditAssociatedServiceDetailsModel(supplier, catalogueItem, solutionMergerAndSplitTypes), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedServiceDetails_InvalidSupplierId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId associatedServiceId,
            [Frozen] ISuppliersService supplierService,
            AssociatedServicesController controller)
        {
            supplierService.GetSupplier(catalogueItemId.SupplierId).Returns((Supplier)null);

            var actual = await controller.EditAssociatedServiceDetails(catalogueItemId, associatedServiceId);

            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Supplier found for Id: {catalogueItemId.SupplierId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedServicesDetails_InvalidAssociatedServiceId_ReturnsBadRequestResult(
            Supplier supplier,
            CatalogueItemId catalogueItemId,
            CatalogueItemId associatedServiceId,
            [Frozen] ISuppliersService supplierService,
            [Frozen] IAssociatedServicesService mockAssociatedService,
            AssociatedServicesController controller)
        {
            supplierService.GetSupplier(catalogueItemId.SupplierId).Returns(supplier);
            mockAssociatedService.GetAssociatedServiceWithCataloguePrices(associatedServiceId).Returns(default(CatalogueItem));

            var actual = await controller.EditAssociatedServiceDetails(catalogueItemId, associatedServiceId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Associated Service found for Id: {associatedServiceId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditAssociatedServiceDetails_Valid_ReturnsViewWithExpectedRouteValues(
            Supplier supplier,
            CatalogueItemId catalogueItemId,
            AssociatedService associatedService,
            [Frozen] ISuppliersService supplierService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            AssociatedServicesController controller)
        {
            var catalogueItem = associatedService.CatalogueItem;

            supplierService.GetSupplier(catalogueItemId.SupplierId).Returns(supplier);
            associatedServicesService.GetAssociatedService(catalogueItem.Id).Returns(catalogueItem);

            var model = new EditAssociatedServiceDetailsModel(supplier, catalogueItem, null);

            var actual = await controller.EditAssociatedServiceDetails(catalogueItemId, catalogueItem.Id, model);

            var result = actual.Should().BeOfType<RedirectToActionResult>().Subject;

            result.ActionName.Should().Be(nameof(AssociatedServicesController.EditAssociatedService));
            result.RouteValues.Should().ContainKey("solutionId").WhoseValue.Should().Be(catalogueItemId);
            result.RouteValues.Should().ContainKey("associatedServiceId").WhoseValue.Should().Be(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditAssociatedServiceDetails_InvalidAssociatedServiceId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId associatedServiceId,
            EditAssociatedServiceDetailsModel model,
            [Frozen] IAssociatedServicesService mockService,
            AssociatedServicesController controller)
        {
            mockService.GetAssociatedService(associatedServiceId).Returns(default(CatalogueItem));

            var actual = await controller.EditAssociatedServiceDetails(catalogueItemId, associatedServiceId, model);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Associated Service found for Id: {associatedServiceId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SetPublicationStatus_CallsSavePublicationStatus(
            CatalogueItem associatedService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            [Frozen] IPublicationStatusService mockPublicationStatusService,
            AssociatedServicesController controller)
        {
            associatedService.PublishedStatus = PublicationStatus.Draft;

            var model = new EditAssociatedServiceModel { SelectedPublicationStatus = PublicationStatus.Published };

            mockAssociatedServicesService.GetAssociatedService(associatedService.Id).Returns(associatedService);

            await controller.SetPublicationStatus(associatedService.Id, associatedService.Id, model);

            await mockPublicationStatusService.Received().SetPublicationStatus(associatedService.Id, model.SelectedPublicationStatus);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SetPublicationStatus_ReturnsRedirectToActionResult(
            CatalogueItem associatedService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            associatedService.PublishedStatus = PublicationStatus.Draft;

            var model = new EditAssociatedServiceModel { SelectedPublicationStatus = PublicationStatus.Published };

            mockAssociatedServicesService.GetAssociatedService(associatedService.Id).Returns(associatedService);

            var actual = (await controller.SetPublicationStatus(associatedService.Id, associatedService.Id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(AssociatedServicesController.AssociatedServices));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SetPublicationStatus_InvalidModel_ReturnsViewWithModel(
            Supplier supplier,
            CatalogueItemId catalogueItemId,
            AssociatedService associatedService,
            [Frozen] ISuppliersService supplierService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            AssociatedServicesController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var model = new EditAssociatedServiceModel(supplier, catalogueItemId, associatedService.CatalogueItem);

            supplierService.GetSupplier(catalogueItemId.SupplierId).Returns(supplier);

            mockAssociatedServicesService.GetAssociatedService(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);

            var actual = (await controller.SetPublicationStatus(catalogueItemId, associatedService.CatalogueItemId, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().Be(nameof(AssociatedServicesController.EditAssociatedService));
            actual.Model.Should().BeEquivalentTo(model);
        }
    }
}
