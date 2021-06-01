using System;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SolutionDetailsControllerTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };

        [Test]
        public static void Class_AreaAttribute_ExpectedAreaName()
        {
            typeof(SolutionDetailsController)
                .GetCustomAttribute<AreaAttribute>()
                .RouteValue.Should()
                .Be("Solutions");
        }

        [Test]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                    () => _ = new SolutionDetailsController(null, Mock.Of<ISolutionsService>()))
                .ParamName.Should()
                .Be("mapper");
        }

        [Test]
        public static void Constructor_NullService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new SolutionDetailsController(Mock.Of<IMapper>(), null))
                .ParamName.Should()
                .Be("solutionsService");
        }

        [Test]
        public static void Get_Description_RouteAttribute_ExpectedTemplate()
        {
            typeof(SolutionDetailsController)
                .GetMethod(nameof(SolutionDetailsController.Description))
                .GetCustomAttribute<RouteAttribute>()
                .Template.Should()
                .Be("solutions/futures/{id}");
        }
        
        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_Description_InvalidId_ThrowsException(string id)
        {
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.Description(id))
                .Message.Should().Be($"{nameof(SolutionDetailsController.Description)}-{nameof(id)}");
        }

        [Test, AutoData]
        public static async Task Get_Description_ValidId_GetsSolutionFromService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                mockService.Object);

            await controller.Description(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_Description_NullSolutionForId_ReturnsBadRequestResult(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.Description(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_Description_ValidSolutionForId_MapsToModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            await controller.Description(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, SolutionDescriptionModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_Description_ValidSolutionForId_ReturnsExpectedViewResult(string id)
        {
            var mockSolutionDescriptionModel = new Mock<SolutionDescriptionModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, SolutionDescriptionModel>(mockCatalogueItem))
                .Returns(mockSolutionDescriptionModel);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            var actual = (await controller.Description(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionDescriptionModel);
        }
        
        [Test]
        public static void Get_ImplementationTimescales_RouteAttribute_ExpectedTemplate()
        {
            typeof(SolutionDetailsController)
                .GetMethod(nameof(SolutionDetailsController.ImplementationTimescales))
                .GetCustomAttribute<RouteAttribute>()
                .Template.Should()
                .Be("solutions/futures/{id}/implementation-timescales");
        }
        
        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_ImplementationTimescales_InvalidId_ThrowsException(string id)
        {
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.ImplementationTimescales(id))
                .Message.Should().Be($"{nameof(SolutionDetailsController.ImplementationTimescales)}-{nameof(id)}");
        }

        [Test, AutoData]
        public static async Task Get_ImplementationTimescales_ValidId_GetsSolutionFromService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                mockService.Object);

            await controller.ImplementationTimescales(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_ImplementationTimescales_NullSolutionForId_ReturnsBadRequestResult(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.ImplementationTimescales(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_ImplementationTimescales_ValidSolutionForId_MapsToModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            await controller.ImplementationTimescales(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, ImplementationTimescalesModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_ImplementationTimescales_ValidSolutionForId_ReturnsExpectedViewResult(string id)
        {
            var mockSolutionImplementationTimescalesModel = new Mock<ImplementationTimescalesModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, ImplementationTimescalesModel>(mockCatalogueItem))
                .Returns(mockSolutionImplementationTimescalesModel);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            var actual = (await controller.ImplementationTimescales(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionImplementationTimescalesModel);
        }
    }
}
