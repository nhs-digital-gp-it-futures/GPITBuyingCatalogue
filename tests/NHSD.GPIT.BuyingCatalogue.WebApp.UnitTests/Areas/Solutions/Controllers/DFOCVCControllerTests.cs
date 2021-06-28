using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    public static class DFOCVCControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(DFOCVCController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Solutions");
        }

        [Fact]
        public static void Constructor_NullSolutionsService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new DFOCVCController(
                null));
        }

        [Fact]
        public static async Task Get_Index_ReturnsDefaultViewWithModelPopulated()
        {
            var solutions = new List<CatalogueItem>
            {
                new() { Name = "Item 1"},
                new() { Name = "Item 2"}
            };

            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(x => x.GetDFOCVCSolutions()).ReturnsAsync(solutions);

            var controller = new DFOCVCController(
                mockSolutionsService.Object);

            var result = await controller.Index();

            Assert.IsAssignableFrom<ViewResult>(result);
            Assert.IsAssignableFrom<SolutionsModel>(((ViewResult)result).Model);
            Assert.Equal(2, ((SolutionsModel)((ViewResult)result).Model).CatalogueItems.Count);
        }
    }
}
