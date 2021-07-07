using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Document;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    public static class FuturesControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(FuturesController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Solutions");
        }

        [Fact]
        public static void Constructor_NullSolutionsService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new FuturesController(
                null,
                Mock.Of<IDocumentService>()));
        }

        [Fact]
        public static void Constructor_NullDocumentService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new FuturesController(
                Mock.Of<ISolutionsService>(),
                null));
        }

        [Fact]
        public static void Get_Index_ReturnsDefaultView()
        {            
            var controller = new FuturesController(
                Mock.Of<ISolutionsService>(), Mock.Of<IDocumentService>());

            var result = controller.Index() as ViewResult;

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        }

        [Fact]
        public static async Task Get_CapabilitiesSelector_ReturnsDefaultView()
        {
            var solutions = new List<Capability>
            {
                new() { Name = "Item 1"},
                new() { Name = "Item 2"},
                new() { Name = "Item 3"},
                new() { Name = "Item 4"},
            };

            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(x => x.GetFuturesCapabilities()).ReturnsAsync(solutions);

            var controller = new FuturesController(
                mockSolutionsService.Object, Mock.Of<IDocumentService>());

            var result = await controller.CapabilitiesSelector() as ViewResult;
            result.Should().NotBeNull();

            var model = result.Model as CapabilitiesModel;
            model.Should().NotBeNull();

            Assert.Equal(2, model.LeftCapabilities.Length);
            Assert.Equal(2, model.RightCapabilities.Length);
            Assert.Equal("./", model.BackLink);
            Assert.Equal("Go back to previous page", model.BackLinkText);
        }

        [Fact]
        public static async Task Get_Foundation_ReturnsDefaultViewWithModelPopulated()
        {
            var solutions = new List<CatalogueItem>
            {
                new() { Name = "Item 1"},
                new() { Name = "Item 2"}
            };

            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(x => x.GetFuturesFoundationSolutions()).ReturnsAsync(solutions);

            var controller = new FuturesController(
                mockSolutionsService.Object, Mock.Of<IDocumentService>());

            var result = await controller.Foundation();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.IsAssignableFrom<SolutionsModel>(((ViewResult)result).Model);
            Assert.Equal(2, ((SolutionsModel)((ViewResult)result).Model).CatalogueItems.Count);
        }
    }
}
