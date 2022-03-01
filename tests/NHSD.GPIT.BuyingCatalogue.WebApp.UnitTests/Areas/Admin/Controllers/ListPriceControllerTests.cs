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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.ListPrices;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class ListPriceControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ListPriceController).Should().BeDecoratedWith<AuthorizeAttribute>(a => a.Policy == "AdminOnly");
            typeof(ListPriceController).Should().BeDecoratedWith<AreaAttribute>(a => a.RouteValue == "Admin");
            typeof(ListPriceController).Should().BeDecoratedWith<RouteAttribute>(r => r.Template == "admin/catalogue-solutions/manage/{solutionId}/list-prices");
        }

        [Fact]
        public static void Constructors_VerifyGuardClauses()
        {
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);
            var constructors = typeof(ListPriceController).GetConstructors();

            assertion.Verify(constructors);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ValidId_ReturnsCataloguePrice(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<IListPricesService> mockListPricesService,
            ListPriceController listPriceController)
        {
            mockListPricesService
                .Setup(s => s.GetCatalogueItemWithPrices(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await listPriceController.ManageListPrices(catalogueItemId)).As<ViewResult>();

            mockListPricesService.Verify(s => s.GetCatalogueItemWithPrices(catalogueItemId));
            actual.ViewName.Should().BeNull();

            var manageListPricesModel = actual.Model.As<ManageListPricesModel>();
            manageListPricesModel.CataloguePrices.Should().BeEquivalentTo(catalogueItem.CataloguePrices);
            manageListPricesModel.CatalogueItemId.Should().Be(catalogueItem.Id);
            manageListPricesModel.CatalogueName.Should().Be(catalogueItem.Name);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<IListPricesService> mockListPricesService,
            ListPriceController listPriceController)
        {
            mockListPricesService
                .Setup(s => s.GetCatalogueItemWithPrices(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await listPriceController.ManageListPrices(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddListPrice_ValidId_ReturnsModel(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<IListPricesService> mockListPricesService,
            ListPriceController listPriceController)
        {
            mockListPricesService
                .Setup(s => s.GetCatalogueItemWithPrices(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = await listPriceController.AddListPrice(catalogueItemId);

            mockListPricesService.VerifyAll();

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();

            var model = actual.As<ViewResult>().Model.As<EditListPriceModel>();
            model.ItemId.Should().Be(catalogueItem.Id);
            model.ItemName.Should().Be(catalogueItem.Name);
            model.IsAdding.Should().BeTrue();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddListPrice_ValidId_WithValidListPriceId_ReturnsModel(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<IListPricesService> mockListPricesService,
            ListPriceController listPriceController)
        {
            var priceToClone = catalogueItem.CataloguePrices.First();

            mockListPricesService
                .Setup(s => s.GetCatalogueItemWithPrices(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = await listPriceController.AddListPrice(catalogueItemId, priceToClone.CataloguePriceId);

            mockListPricesService.VerifyAll();

            actual.Should().NotBeNull();
            actual.Should().BeOfType<ViewResult>();

            var model = actual.As<ViewResult>().Model.As<EditListPriceModel>();

            model.ItemId.Should().Be(catalogueItem.Id);
            model.ItemName.Should().Be(catalogueItem.Name);
            model.IsAdding.Should().BeTrue();
            model.CataloguePriceId.Should().BeNull();

            model.CatalogueItemType.Should().Be(catalogueItem.CatalogueItemType);
            model.Price.Should().Be(priceToClone.Price);
            model.Unit.Should().Be(priceToClone.PricingUnit.Description);
            model.UnitDefinition.Should().Be(priceToClone.PricingUnit.Definition);
            model.SelectedProvisioningType.Should().Be(priceToClone.ProvisioningType);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AddListPrice_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<IListPricesService> mockListPricesService,
            ListPriceController listPriceController)
        {
            mockListPricesService
                .Setup(s => s.GetCatalogueItemWithPrices(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await listPriceController.AddListPrice(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AddListPrice_ModelStateValid_RedirectsToPublishListPrice(
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            [Frozen] Mock<IListPricesService> mockListPricesService,
            ListPriceController listPriceController)
        {
            const decimal price = 3.21M;
            var solutionId = catalogueItem.Id;
            var editListPriceModel = new EditListPriceModel(catalogueItem)
            {
                Price = price,
                SelectedProvisioningType = ProvisioningType.Patient,
                Unit = "per patient",
            };

            mockSolutionsService
                .Setup(s => s.GetSolutionThin(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = await listPriceController.AddListPrice(solutionId, editListPriceModel);

            mockListPricesService.Verify(s => s.SaveListPrice(catalogueItem.Id, It.IsAny<SaveListPriceModel>()));

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ControllerName.Should().BeNull();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(ListPriceController.PublishListPrice));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditListPrice_ValidId_ReturnsModel(
            CatalogueItem catalogueItem,
            [Frozen] Mock<IListPricesService> mockListPriceService,
            ListPriceController listPriceController)
        {
            var cataloguePriceId = catalogueItem
                .CataloguePrices
                .First()
                .CataloguePriceId;

            catalogueItem.CataloguePrices.First().IsLocked = false;

            mockListPriceService
                .Setup(s => s.GetCatalogueItemWithPrices(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var actual = await listPriceController.EditListPrice(catalogueItem.Id, cataloguePriceId);

            mockListPriceService.Verify(s => s.GetCatalogueItemWithPrices(catalogueItem.Id));
            actual.Should().NotBeNull();
            actual.As<ViewResult>().Model.Should().BeOfType<EditListPriceModel>();

            var model = actual.As<ViewResult>().Model.As<EditListPriceModel>();
            model.ItemName.Should().Be(catalogueItem.Name);
            model.CataloguePriceId.Should().Be(cataloguePriceId);
            model.IsAdding.Should().BeFalse();
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_EditListPrice_InvalidListPriceId_RedirectsToManageListPrices(
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            ListPriceController listPriceController)
        {
            const int cataloguePriceId = int.MaxValue;

            mockSolutionsService
               .Setup(s => s.GetSolutionThin(catalogueItem.Id))
               .ReturnsAsync(catalogueItem);

            var actual = await listPriceController.EditListPrice(catalogueItem.Id, cataloguePriceId);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ControllerName.Should().BeNull();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(ListPriceController.ManageListPrices));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_EditListPrice_ModelStateValid_RedirectsToPublishListPrice(
            CatalogueItem catalogueItem,
            [Frozen] Mock<IListPricesService> mockListPricesService,
            ListPriceController listPriceController)
        {
            const decimal price = 3.21M;
            var solutionId = catalogueItem.Id;
            var cataloguePriceId = catalogueItem
                .CataloguePrices
                .First()
                .CataloguePriceId;

            catalogueItem.CataloguePrices.First().IsLocked = false;

            mockListPricesService.Setup(lps => lps.GetCatalogueItemWithPrices(catalogueItem.Id))
                .ReturnsAsync(catalogueItem);

            var editListPriceModel = new EditListPriceModel(catalogueItem)
            {
                CataloguePriceId = cataloguePriceId,
                Price = price,
                SelectedProvisioningType = ProvisioningType.Patient,
                Unit = "per patient",
            };

            var actual = await listPriceController.EditListPrice(solutionId, cataloguePriceId, editListPriceModel);

            mockListPricesService.Verify(s => s.UpdateListPrice(catalogueItem.Id, It.IsAny<SaveListPriceModel>()));

            actual.Should().NotBeNull();
            actual.Should().BeOfType<RedirectToActionResult>();
            actual.As<RedirectToActionResult>().ControllerName.Should().BeNull();
            actual.As<RedirectToActionResult>().ActionName.Should().Be(nameof(ListPriceController.PublishListPrice));
        }
    }
}
