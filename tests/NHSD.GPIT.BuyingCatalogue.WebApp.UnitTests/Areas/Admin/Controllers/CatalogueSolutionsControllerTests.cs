using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class CatalogueSolutionsControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(CatalogueSolutionsController).Should()
                .BeDecoratedWith<AuthorizeAttribute>(x => x.Policy == "AdminOnly");
            typeof(CatalogueSolutionsController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Admin");
            typeof(CatalogueSolutionsController).Should()
                .BeDecoratedWith<RouteAttribute>(x => x.Template == "admin/catalogue-solutions");
        }

        [Fact]
        public static void Constructor_NullService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                    () =>
                        _ = new CatalogueSolutionsController(null, Mock.Of<IMapper>()))
                .ParamName.Should()
                .Be("solutionsService");
        }

        [Fact]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                    () =>
                        _ = new CatalogueSolutionsController(
                            Mock.Of<ISolutionsService>(),
                            null))
                .ParamName.Should()
                .Be("mapper");
        }

        [Fact]
        public static void Get_Index_HttpGetAttribute_Present()
        {
            typeof(CatalogueSolutionsController)
                .GetMethod(nameof(CatalogueSolutionsController.Index))
                .GetCustomAttribute<HttpGetAttribute>()
                .Should()
                .NotBeNull();
        }

        [Fact]
        public static async Task Get_Index_GetsAllSolutions()
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new CatalogueSolutionsController(mockSolutionsService.Object, Mock.Of<IMapper>());

            await controller.Index();
            
            mockSolutionsService.Verify(s => s.GetAllSolutions());
        }

        [Fact]
        public static async Task Get_Index_MapsAllSolutions()
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var solutions = new Mock<IList<CatalogueItem>>().Object;
            mockSolutionsService.Setup(s => s.GetAllSolutions())
                .ReturnsAsync(solutions);
            
            var mockMapper = new Mock<IMapper>();
            var controller = new CatalogueSolutionsController(mockSolutionsService.Object, mockMapper.Object);

            await controller.Index();
            
            mockMapper.Verify(m => m.Map<IList<CatalogueItem>, CatalogueSolutionsModel>(solutions));
        }

        [Fact]
        public static async Task Get_Index_ReturnsViewWithExpectedModel()
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var solutions = new Mock<IList<CatalogueItem>>().Object;
            mockSolutionsService.Setup(s => s.GetAllSolutions())
                .ReturnsAsync(solutions);
            
            var mockMapper = new Mock<IMapper>();
            var mockCatalogueSolutionsModel = new Mock<CatalogueSolutionsModel>().Object;
            mockMapper.Setup(m => m.Map<IList<CatalogueItem>, CatalogueSolutionsModel>(solutions))
                .Returns(mockCatalogueSolutionsModel);
            var controller = new CatalogueSolutionsController(mockSolutionsService.Object, mockMapper.Object);

            var actual = (await controller.Index()).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.Model.Should().Be(mockCatalogueSolutionsModel);
        }
    }
}
