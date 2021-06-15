using System;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
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
        public static void Get_ClientApplicationTypes_RouteAttribute_ExpectedTemplate()
        {
            typeof(SolutionDetailsController)
                .GetMethod(nameof(SolutionDetailsController.ClientApplicationTypes))
                .GetCustomAttribute<RouteAttribute>()
                .Template.Should()
                .Be("solutions/futures/{id}/client-application-types");
        }
        
        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_ClientApplicationTypes_InvalidId_ThrowsException(string id)
        {
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.ClientApplicationTypes(id))
                .Message.Should().Be($"{nameof(SolutionDetailsController.ClientApplicationTypes)}-{nameof(id)}");
        }

        [Test, AutoData]
        public static async Task Get_ClientApplicationTypes_ValidId_GetsSolutionFromService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                mockService.Object);

            await controller.ClientApplicationTypes(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_ClientApplicationTypes_NullSolutionForId_ReturnsBadRequestResult(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.ClientApplicationTypes(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_ClientApplicationTypes_ValidSolutionForId_MapsToModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            await controller.ClientApplicationTypes(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, ClientApplicationTypesModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_ClientApplicationTypes_ValidSolutionForId_ReturnsExpectedViewResult(string id)
        {
            var mockSolutionClientApplicationTypesModel = new Mock<ClientApplicationTypesModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, ClientApplicationTypesModel>(mockCatalogueItem))
                .Returns(mockSolutionClientApplicationTypesModel);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            var actual = (await controller.ClientApplicationTypes(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionClientApplicationTypesModel);
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
        public static void Get_Features_RouteAttribute_ExpectedTemplate()
        {
            typeof(SolutionDetailsController)
                .GetMethod(nameof(SolutionDetailsController.Features))
                .GetCustomAttribute<RouteAttribute>()
                .Template.Should()
                .Be("solutions/futures/{id}/features");
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_Features_InvalidId_ThrowsException(string id)
        {
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.Features(id));
        }

        [Test, AutoData]
        public static async Task Get_Features_ValidId_InvokesGetSolution(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                mockService.Object);

            await controller.Features(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_Features_NullSolutionForId_ReturnsBadRequestResult(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.Features(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_Features_ValidSolutionForId_MapsToModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            await controller.Features(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, SolutionFeaturesModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_Features_ValidSolutionForId_ReturnsExpectedViewResult(string id)
        {
            var mockSolutionFeaturesModel = new Mock<SolutionFeaturesModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, SolutionFeaturesModel>(mockCatalogueItem))
                .Returns(mockSolutionFeaturesModel);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            var actual = (await controller.Features(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionFeaturesModel);
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

        [Test]
        public static void Get_HostingType_RouteAttribute_ExpectedTemplate()
        {
            typeof(SolutionDetailsController)
                .GetMethod(nameof(SolutionDetailsController.HostingType))
                .GetCustomAttribute<RouteAttribute>()
                .Template.Should()
                .Be("solutions/futures/{id}/hosting-type");
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_HostingType_InvalidId_ThrowsException(string id)
        {
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.HostingType(id))
                .Message.Should().Be($"{nameof(SolutionDetailsController.HostingType)}-{nameof(id)}");
        }

        [Test, AutoData]
        public static async Task Get_HostingType_ValidId_GetsSolutionFromService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                mockService.Object);

            await controller.HostingType(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_HostingType_NullSolutionForId_ReturnsBadRequestResult(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.HostingType(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_HostingType_ValidSolutionForId_MapsToModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            await controller.HostingType(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, HostingTypesModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_HostingType_ValidSolutionForId_ReturnsExpectedViewResult(string id)
        {
            var mockSolutionHostingTypeModel = new Mock<HostingTypesModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, HostingTypesModel>(mockCatalogueItem))
                .Returns(mockSolutionHostingTypeModel);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            var actual = (await controller.HostingType(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionHostingTypeModel);
        }

        [Test]
        public static void Get_ListPrice_RouteAttribute_ExpectedTemplate()
        {
            typeof(SolutionDetailsController)
                .GetMethod(nameof(SolutionDetailsController.ListPrice))
                .GetCustomAttribute<RouteAttribute>()
                .Template.Should()
                .Be("solutions/futures/{id}/list-price");
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_ListPrice_InvalidId_ThrowsException(string id)
        {
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.ListPrice(id))
                .Message.Should().Be($"{nameof(SolutionDetailsController.ListPrice)}-{nameof(id)}");
        }

        [Test, AutoData]
        public static async Task Get_ListPrice_ValidId_GetsSolutionFromService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                mockService.Object);

            await controller.ListPrice(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_ListPrice_NullSolutionForId_ReturnsBadRequestResult(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.ListPrice(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_ListPrice_ValidSolutionForId_MapsToModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            await controller.ListPrice(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, ListPriceModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_ListPrice_ValidSolutionForId_ReturnsExpectedViewResult(string id)
        {
            var mockSolutionListPriceModel = new Mock<ListPriceModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, ListPriceModel>(mockCatalogueItem))
                .Returns(mockSolutionListPriceModel);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            var actual = (await controller.ListPrice(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionListPriceModel);
        }

        [Test]
        public static void Get_Capabilities_RouteAttribute_ExpectedTemplate()
        {
            typeof(SolutionDetailsController)
                .GetMethod(nameof(SolutionDetailsController.Capabilities))
                .GetCustomAttribute<RouteAttribute>()
                .Template.Should()
                .Be("solutions/futures/{id}/capabilities");
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_Capabilities_InvalidId_ThrowsException(string id)
        {
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.Capabilities(id))
                .Message.Should().Be($"{nameof(SolutionDetailsController.Capabilities)}-{nameof(id)}");
        }

        [Test, AutoData]
        public static async Task Get_Capabilities_ValidId_GetsSolutionFromService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                mockService.Object);

            await controller.Capabilities(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_Capabilities_NullSolutionForId_ReturnsBadRequestResult(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.Capabilities(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        public static async Task Get_Capabilities_ValidSolutionForId_MapsToModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            await controller.Capabilities(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, CapabilitiesViewModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_Capabilities_ValidSolutionForId_ReturnsExpectedViewResult(string id)
        {
            var mockCapabilitiesViewModel = new Mock<CapabilitiesViewModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, CapabilitiesViewModel>(mockCatalogueItem))
                .Returns(mockCapabilitiesViewModel);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            var actual = (await controller.Capabilities(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockCapabilitiesViewModel);
        }
    }
}
