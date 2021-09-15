using System;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
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
            typeof(ListPriceController).Should().BeDecoratedWith<RouteAttribute>(r => r.Template == "admin/catalogue-solutions");
        }

        [Fact]
        public static void Constructor_NullSolutionsService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new ListPriceController(null));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ValidId_ReturnsCataloguePrice(
            CatalogueItemId catalogueItemId,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            ListPriceController listPriceController)
        {
            mockSolutionsService
                .Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(catalogueItem);

            var actual = (await listPriceController.Index(catalogueItemId)).As<ViewResult>();

            mockSolutionsService.Verify(s => s.GetSolution(catalogueItemId));
            actual.ViewName.Should().BeNull();

            var manageListPricesModel = actual.Model.As<ManageListPricesModel>();
            manageListPricesModel.CataloguePrices.Should().BeEquivalentTo(catalogueItem.CataloguePrices);
            manageListPricesModel.CatalogueItemId.Should().BeEquivalentTo(catalogueItem.Id);
            manageListPricesModel.CatalogueName.Should().BeEquivalentTo(catalogueItem.Name);
            manageListPricesModel.BackLinkText.Should().BeEquivalentTo("Go back");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_InvalidId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            ListPriceController listPriceController)
        {
            mockSolutionsService
                .Setup(s => s.GetSolution(catalogueItemId))
                .ReturnsAsync(default(CatalogueItem));

            var actual = (await listPriceController.Index(catalogueItemId)).As<BadRequestObjectResult>();

            actual.Value.Should().Be($"No Solution found for Id: {catalogueItemId}");
        }
    }
}
