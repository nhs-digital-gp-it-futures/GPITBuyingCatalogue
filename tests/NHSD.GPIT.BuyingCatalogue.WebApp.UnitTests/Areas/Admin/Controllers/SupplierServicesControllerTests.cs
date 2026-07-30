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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.SupplierServices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class SupplierServicesControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(SupplierServicesController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "ManageSupplierOrganisations");
            typeof(SupplierServicesController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Admin");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(SupplierServicesController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AssociatedServices_ValidId_ReturnsViewWithExpectedModel(
            Supplier supplier,
            List<AssociatedService> associatedServices,
            [Frozen] ISuppliersService suppliersService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            SupplierServicesController controller)
        {
            suppliersService.GetSupplier(supplier.Id).Returns(supplier);

            mockAssociatedServicesService.GetAllAssociatedServicesForSupplier(supplier.Id).Returns(associatedServices);

            var actual = await controller.AssociatedServices(supplier.Id);

            actual.Should().BeOfType<ViewResult>();

            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(new AssociatedServicesModel(supplier, associatedServices), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AssociatedServices_InvalidId_ReturnsBadRequestResult(
            int supplierId,
            [Frozen] ISuppliersService suppliersService,
            SupplierServicesController controller)
        {
            suppliersService.GetSupplier(supplierId).Returns((Supplier)null);

            var actual = await controller.AssociatedServices(supplierId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Supplier found for Id: {supplierId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddAssociatedService_ValidId_ReturnsViewWithExpectedModel(
            Supplier supplier,
            [Frozen] ISuppliersService supplierService,
            SupplierServicesController controller)
        {
            supplierService.GetSupplier(supplier.Id).Returns(supplier);

            var actual = await controller.AddAssociatedService(supplier.Id);

            actual.Should().BeOfType<ViewResult>();

            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(new AddAssociatedServiceModel(supplier), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_AddAssociatedService_InvalidId_ReturnsBadRequestResult(
            int supplierId,
            [Frozen] ISuppliersService supplierService,
            SupplierServicesController controller)
        {
            supplierService.GetSupplier(supplierId).Returns((Supplier)null);

            var actual = await controller.AddAssociatedService(supplierId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Supplier found for Id: {supplierId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_AddAssociatedService_Redirects(
            int supplierId,
            CatalogueItemId associatedServiceId,
            AddAssociatedServiceModel model,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            SupplierServicesController controller)
        {
            mockAssociatedServicesService.AddAssociatedService(supplierId, Arg.Any<AssociatedServicesDetailsModel>()).Returns(associatedServiceId);

            var actual = await controller.AddAssociatedService(supplierId, model);

            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(SupplierServicesController.EditAssociatedService));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedService_ValidIds_ReturnsViewWithExpectedModel(
            Supplier supplier,
            AssociatedService associatedService,
            List<CataloguePrice> listPrices,
            [Frozen] ISuppliersService supplierService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            SupplierServicesController controller)
        {
            associatedService.CatalogueItem.PublishedStatus = PublicationStatus.Draft;
            associatedService.CatalogueItem.CataloguePrices.AddRange(listPrices);

            supplierService.GetSupplier(supplier.Id).Returns(supplier);

            mockAssociatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId).Returns(associatedService.CatalogueItem);
            mockAssociatedServicesService.GetAssociatedServiceReferences(associatedService.CatalogueItemId)
                .Returns(Enumerable.Empty<CatalogueItem>().ToList());

            var expectedModel = new EditAssociatedServiceModel(supplier, associatedService.CatalogueItem, []);

            var actual = await controller.EditAssociatedService(supplier.Id, associatedService.CatalogueItemId);

            await supplierService.Received().GetSupplier(supplier.Id);
            await mockAssociatedServicesService.Received().GetAssociatedServiceWithCataloguePrices(associatedService.CatalogueItemId);

            actual.Should().BeOfType<ViewResult>();

            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(expectedModel, opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedService_InvalidSupplierId_ReturnsBadRequestResult(
            int supplierId,
            CatalogueItemId associatedServiceId,
            [Frozen] ISuppliersService supplierService,
            SupplierServicesController controller)
        {
            supplierService.GetSupplier(supplierId).Returns((Supplier)null);

            var actual = await controller.EditAssociatedService(supplierId, associatedServiceId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Supplier found for Id: {supplierId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedService_InvalidAssociatedServiceId_ReturnsBadRequestResult(
            Supplier supplier,
            CatalogueItemId associatedServiceId,
            [Frozen] ISuppliersService supplierService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            SupplierServicesController controller)
        {
            supplierService.GetSupplier(supplier.Id).Returns(supplier);

            mockAssociatedServicesService.GetAssociatedServiceWithCataloguePrices(associatedServiceId).Returns(default(CatalogueItem));

            var actual = await controller.EditAssociatedService(supplier.Id, associatedServiceId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No associated service found for Id: {associatedServiceId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedServiceDetails_ValidIds_ReturnsViewWithExpectedModel(
            Supplier supplier,
            AssociatedService associatedService,
            List<SolutionMergerAndSplitTypesModel> solutionMergerAndSplitTypes,
            [Frozen] ISuppliersService suppliersService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            SupplierServicesController controller)
        {
            suppliersService.GetSupplier(supplier.Id).Returns(supplier);

            var catalogueItem = associatedService.CatalogueItem;
            mockAssociatedServicesService.GetAssociatedServiceWithCataloguePrices(catalogueItem.Id).Returns(catalogueItem);

            mockAssociatedServicesService.GetSolutionsWithMergerAndSplitTypesForButExcludingAssociatedService(catalogueItem.Id).Returns(solutionMergerAndSplitTypes);

            var actual = await controller.EditAssociatedServiceDetails(supplier.Id, catalogueItem.Id);

            await suppliersService.Received().GetSupplier(supplier.Id);
            await mockAssociatedServicesService.Received().GetAssociatedServiceWithCataloguePrices(catalogueItem.Id);

            actual.Should().BeOfType<ViewResult>();

            actual.As<ViewResult>().ViewName.Should().BeNull();
            actual.As<ViewResult>().Model.Should().BeEquivalentTo(new EditAssociatedServiceDetailsModel(supplier, catalogueItem, solutionMergerAndSplitTypes), opt => opt.Excluding(m => m.BackLink));
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedServiceDetails_InvalidSupplierId_ReturnsBadRequestResult(
            int supplierId,
            CatalogueItemId associatedServiceId,
            [Frozen] ISuppliersService supplierService,
            SupplierServicesController controller)
        {
            supplierService.GetSupplier(supplierId).Returns((Supplier)null);

            var actual = await controller.EditAssociatedServiceDetails(supplierId, associatedServiceId);

            actual.Should().BeOfType<BadRequestObjectResult>();
            actual.As<BadRequestObjectResult>().Value.Should().Be($"No Supplier found for Id: {supplierId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Get_EditAssociatedServicesDetails_InvalidAssociatedServiceId_ReturnsBadRequestResult(
            Supplier supplier,
            CatalogueItemId associatedServiceId,
            [Frozen] ISuppliersService supplierService,
            [Frozen] IAssociatedServicesService mockAssociatedService,
            SupplierServicesController controller)
        {
            supplierService.GetSupplier(supplier.Id).Returns(supplier);
            mockAssociatedService.GetAssociatedServiceWithCataloguePrices(associatedServiceId).Returns(default(CatalogueItem));

            var actual = await controller.EditAssociatedServiceDetails(supplier.Id, associatedServiceId);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No associated service found for Id: {associatedServiceId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditAssociatedServiceDetails_Valid_ReturnsViewWithExpectedRouteValues(
            Supplier supplier,
            AssociatedService associatedService,
            [Frozen] ISuppliersService supplierService,
            [Frozen] IAssociatedServicesService associatedServicesService,
            SupplierServicesController controller)
        {
            var catalogueItem = associatedService.CatalogueItem;

            supplierService.GetSupplier(supplier.Id).Returns(supplier);
            associatedServicesService.GetAssociatedService(catalogueItem.Id).Returns(catalogueItem);

            var model = new EditAssociatedServiceDetailsModel(supplier, catalogueItem, null);

            var actual = await controller.EditAssociatedServiceDetails(supplier.Id, catalogueItem.Id, model);

            var result = actual.Should().BeOfType<RedirectToActionResult>().Subject;

            result.ActionName.Should().Be(nameof(SupplierServicesController.EditAssociatedService));
            result.RouteValues.Should().ContainKey("supplierId").WhoseValue.Should().Be(supplier.Id);
            result.RouteValues.Should().ContainKey("associatedServiceId").WhoseValue.Should().Be(catalogueItem.Id);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_EditAssociatedServiceDetails_InvalidAssociatedServiceId_ReturnsBadRequestResult(
            int supplierId,
            CatalogueItemId associatedServiceId,
            EditAssociatedServiceDetailsModel model,
            [Frozen] IAssociatedServicesService mockService,
            SupplierServicesController controller)
        {
            mockService.GetAssociatedService(associatedServiceId).Returns(default(CatalogueItem));

            var actual = await controller.EditAssociatedServiceDetails(supplierId, associatedServiceId, model);

            actual.Should().BeOfType<BadRequestObjectResult>();

            actual.As<BadRequestObjectResult>().Value.Should().Be($"No associated service found for Id: {associatedServiceId}");
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SetPublicationStatus_CallsSavePublicationStatus(
            int supplierId,
            CatalogueItem associatedService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            [Frozen] IPublicationStatusService mockPublicationStatusService,
            SupplierServicesController controller)
        {
            associatedService.PublishedStatus = PublicationStatus.Draft;

            var model = new EditAssociatedServiceModel { SelectedPublicationStatus = PublicationStatus.Published };

            mockAssociatedServicesService.GetAssociatedService(associatedService.Id).Returns(associatedService);

            await controller.SetPublicationStatus(supplierId, associatedService.Id, model);

            await mockPublicationStatusService.Received().SetPublicationStatus(associatedService.Id, model.SelectedPublicationStatus);
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SetPublicationStatus_ReturnsRedirectToActionResult(
            int supplierId,
            CatalogueItem associatedService,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            SupplierServicesController controller)
        {
            associatedService.PublishedStatus = PublicationStatus.Draft;

            var model = new EditAssociatedServiceModel { SelectedPublicationStatus = PublicationStatus.Published };

            mockAssociatedServicesService.GetAssociatedService(associatedService.Id).Returns(associatedService);

            var actual = (await controller.SetPublicationStatus(supplierId, associatedService.Id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SupplierServicesController.AssociatedServices));
        }

        [Theory]
        [MockAutoData]
        public static async Task Post_SetPublicationStatus_InvalidModel_ReturnsViewWithModel(
            Supplier supplier,
            AssociatedService associatedService,
            List<Solution> solutions,
            List<AdditionalService> additionalServices,
            [Frozen] IAssociatedServicesService mockAssociatedServicesService,
            SupplierServicesController controller)
        {
            controller.ModelState.AddModelError("some-key", "some-error");

            var referencedItems = solutions.Select(x => x.CatalogueItem)
                .Concat(additionalServices.Select(x => x.CatalogueItem))
                .ToList();

            var model = new EditAssociatedServiceModel(supplier, associatedService.CatalogueItem, referencedItems);

            mockAssociatedServicesService.GetAssociatedServiceReferences(associatedService.CatalogueItemId).Returns(referencedItems);

            var actual = (await controller.SetPublicationStatus(supplier.Id, associatedService.CatalogueItemId, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().Be(nameof(SupplierServicesController.EditAssociatedService));
            actual.Model.Should().BeEquivalentTo(model);
        }
    }
}
