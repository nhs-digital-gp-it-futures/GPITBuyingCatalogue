using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;
using PublicationStatus = NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue.PublicationStatus;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class AddCatalogueSolutionControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AddCatalogueSolutionController).Should()
                .BeDecoratedWith<AuthorizeAttribute>(x => x.Policy == "AdminOnly");
            
            typeof(AddCatalogueSolutionController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Admin");
            
            typeof(AddCatalogueSolutionController).Should()
                .BeDecoratedWith<RouteAttribute>(x => x.Template == "admin/catalogue-solutions/add-solution");
        }

        [Fact]
        public static void Constructor_NullService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                    () =>
                        _ = new AddCatalogueSolutionController(null))
                .ParamName.Should()
                .Be("solutionsService");
        }
        
        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_SuppliersInDatabase_ReturnsViewWithExpectedModel(
            List<Supplier> suppliers,
            Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetAllSuppliers())
                .ReturnsAsync(suppliers);
            var controller = new AddCatalogueSolutionController(mockService.Object);

            var actual = (await controller.Index()).As<ViewResult>();

            mockService.Verify(s => s.GetAllSuppliers());
            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.As<AddSolutionModel>()
                .Suppliers.Should()
                .BeEquivalentTo(suppliers?.ToDictionary(s => s.Id, s => s.Name));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Index_ModelStateValid_GetsLatestCatalogueItemId(
            AddSolutionModel model,
            Mock<ISolutionsService> mockService)
        {
            mockService.Setup(s => s.GetLatestCatalogueItemIdFor(model.SupplierId))
                .ReturnsAsync(new CatalogueItemId(int.Parse(model.SupplierId), "012"));
            var controller = new AddCatalogueSolutionController(mockService.Object);

            await controller.Index(model);

            mockService.Verify(s => s.GetLatestCatalogueItemIdFor(model.SupplierId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Index_ModelStateValid_AddsExpectedCatalogueItem(
            AddSolutionModel model,
            Mock<ISolutionsService> mockService)
        {
            var catalogueItemId = new CatalogueItemId(int.Parse(model.SupplierId), "012");
            mockService.Setup(s => s.GetLatestCatalogueItemIdFor(model.SupplierId))
                .ReturnsAsync(catalogueItemId);
            var controller = new AddCatalogueSolutionController(mockService.Object);

            await controller.Index(model);

            mockService.Verify(
                s => s.AddCatalogueSolution(
                    It.Is<CatalogueItem>(
                        c => c.CatalogueItemId == catalogueItemId.NextSolutionId()
                            && c.CatalogueItemType == CatalogueItemType.Solution
                            && c.Name == model.SolutionName
                            && c.PublishedStatus == PublicationStatus.Draft
                            && c.SupplierId == model.SupplierId)));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Index_ModelStateValid_RedirectsToCatalogueSolutions(
            AddSolutionModel model,
            Mock<ISolutionsService> mockService)
        {
            var catalogueItemId = new CatalogueItemId(int.Parse(model.SupplierId), "012");
            mockService.Setup(s => s.GetLatestCatalogueItemIdFor(model.SupplierId))
                .ReturnsAsync(catalogueItemId);
            var controller = new AddCatalogueSolutionController(mockService.Object);

            var actual = (await controller.Index(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(CatalogueSolutionsController.Index));
            actual.ControllerName.Should().Be(typeof(CatalogueSolutionsController).ControllerName());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Index_ModelStateNotValid_GetsSuppliers_ReturnsViewWithModel(
            AddSolutionModel model,
            Mock<ISolutionsService> mockService,
            List<Supplier> suppliers)
        {
            mockService.Setup(s => s.GetAllSuppliers())
                .ReturnsAsync(suppliers);
            var controller = new AddCatalogueSolutionController(mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Index(model)).As<ViewResult>();

            mockService.Verify(s => s.GetAllSuppliers());
            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().BeEquivalentTo(model.WithSuppliers(suppliers));
        }
    }
}
