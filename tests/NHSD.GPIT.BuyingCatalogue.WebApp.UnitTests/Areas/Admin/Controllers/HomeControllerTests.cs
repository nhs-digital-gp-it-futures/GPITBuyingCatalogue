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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Admin.Controllers
{
    public static class HomeControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(HomeController).Should().BeDecoratedWith<AuthorizeAttribute>(x => x.Policy == "AdminOnly");
            typeof(HomeController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Admin");
            typeof(HomeController).Should().BeDecoratedWith<RouteAttribute>(x => x.Template == "admin");
        }

        [Fact]
        public static void Constructor_NullOrganisationService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                    () =>
                        _ = new HomeController(null, Mock.Of<IMapper>()))
                .ParamName.Should()
                .Be("organisationsService");
        }

        [Fact]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                    () =>
                        _ = new HomeController(
                            Mock.Of<IOrganisationsService>(),
                            null))
                .ParamName.Should()
                .Be("mapper");
        }

        [Fact]
        public static void Constructor_NullSolutionsService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                    () =>
                        _ = new HomeController(
                            Mock.Of<IOrganisationsService>(),
                            Mock.Of<IMapper>()))
                .ParamName.Should()
                .Be("solutionsService");
        }

        [Fact]
        public static void Get_BuyerOrganisations_RouteAttribute_ExpectedTemplate()
        {
            typeof(HomeController)
                .GetMethod(nameof(HomeController.BuyerOrganisations))
                .GetCustomAttribute<RouteAttribute>()
                .Template.Should()
                .Be("buyer-organisations");
        }

        [Fact]
        public static async Task Get_BuyerOrganisations_GetsAllOrganisations()
        {
            var mockOrganisationService = new Mock<IOrganisationsService>();
            var controller = new HomeController(                
                mockOrganisationService.Object,
                Mock.Of<IMapper>());

            await controller.BuyerOrganisations();

            mockOrganisationService.Verify(o => o.GetAllOrganisations());
        }

        [Fact]
        public static async Task Get_BuyerOrganisations_MapsOrganisationsToModels()
        {
            var mockOrganisationService = new Mock<IOrganisationsService>();
            var mockOrganisations = new Mock<IList<Organisation>>().Object;
            mockOrganisationService.Setup(o => o.GetAllOrganisations())
                .ReturnsAsync(mockOrganisations);
            var mockMapper = new Mock<IMapper>();
            var controller = new HomeController(                
                mockOrganisationService.Object,
                mockMapper.Object);

            await controller.BuyerOrganisations();

            mockMapper.Verify(m => m.Map<IList<Organisation>, IList<OrganisationModel>>(mockOrganisations));
        }

        [Fact]
        public static async Task Get_BuyerOrganisations_ReturnsViewWithExpectedViewModel()
        {
            var mockOrganisationService = new Mock<IOrganisationsService>();
            var mockOrganisations = new Mock<IList<Organisation>>().Object;
            mockOrganisationService.Setup(o => o.GetAllOrganisations())
                .ReturnsAsync(mockOrganisations);
            var mockMapper = new Mock<IMapper>();
            var mockOrganisationModels = new Mock<IList<OrganisationModel>>().Object;
            mockMapper.Setup(m => m.Map<IList<Organisation>, IList<OrganisationModel>>(mockOrganisations))
                .Returns(mockOrganisationModels);
            var controller = new HomeController(                
                mockOrganisationService.Object,
                mockMapper.Object);

            var actual = (await controller.BuyerOrganisations()).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            Assert.Same(mockOrganisationModels, actual.Model);
        }

        [Fact]
        public static void Get_Index_ReturnsDefaultView()
        {
            var controller = new HomeController(
                Mock.Of<IOrganisationsService>(),
                Mock.Of<IMapper>());

            var result = controller.Index().As<ViewResult>();

            result.Should().NotBeNull();
            result.ViewName.Should().BeNull();
        }

        [Fact]
        public static async Task Get_AddSolution_GetAllSuppliers()
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new HomeController(
                Mock.Of<IOrganisationsService>(),
                Mock.Of<IMapper>());

            await controller.AddSolution();

            mockSolutionsService.Verify(o => o.GetAllSuppliers());
        }

        [Fact]
        public static async Task Get_AddSolution_ReturnsViewWithExpectedViewModel()
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockSuppliers = new Mock<IList<Supplier>>().Object;
            mockSolutionsService.Setup(o => o.GetAllSuppliers())
                .ReturnsAsync(mockSuppliers);
            var controller = new HomeController(
                Mock.Of<IOrganisationsService>(),
                Mock.Of<IMapper>());

            var actual = (await controller.AddSolution()).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
        }
    }
}
