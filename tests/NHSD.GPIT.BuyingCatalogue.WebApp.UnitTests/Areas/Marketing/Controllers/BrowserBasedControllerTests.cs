using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class BrowserBasedControllerTests
    {
        private static string[] InvalidStrings = { null, string.Empty, "    " };

        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(BrowserBasedController).Should()
                .BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
            typeof(BrowserBasedController).Should()
                .BeDecoratedWith<RouteAttribute>(x =>
                    x.Template == "marketing/supplier/solution/{id}/section/browser-based");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new BrowserBasedController(null, Mock.Of<IMapper>(), Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), null,
                    Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), Mock.Of<IMapper>(),
                    null));
        }

        [Test]
        public static void Get_SupportedBrowsers_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(BrowserBasedController)
                .GetMethods()
                .First(x => x.Name == nameof(BrowserBasedController.SupportedBrowsers)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be("supported-browsers");
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_SupportedBrowsers_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), 
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.SupportedBrowsers(id));
        }

        [Test, AutoData]
        public static async Task Get_SupportedBrowsers_ValidId_CallsGetSolutionOnService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), 
                Mock.Of<IMapper>(), mockService.Object);

            await controller.SupportedBrowsers(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_SupportedBrowsers_NullSolutionFromService_ReturnsBadRequestResponse(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.SupportedBrowsers(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }
        
        [Test, AutoData]
        public static async Task Get_SupportedBrowsers_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                mockMapper.Object, mockService.Object);

            await controller.SupportedBrowsers(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, SupportedBrowsersModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_SupportedBrowsers_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockSupportedBrowsersModel = new Mock<SupportedBrowsersModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, SupportedBrowsersModel>(mockCatalogueItem))
                .Returns(mockSupportedBrowsersModel);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.SupportedBrowsers(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockSupportedBrowsersModel);
        }

        [Test]
        public static void Post_SupportedBrowsers_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(BrowserBasedController)
                .GetMethods()
                .First(x => x.Name == nameof(BrowserBasedController.SupportedBrowsers)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be("supported-browsers");
        }

        [Test]
        public static void Post_SupportedBrowsers_NullModel_ThrowsException()
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            var actual = Assert.ThrowsAsync<ArgumentNullException>(() =>
                controller.SupportedBrowsers(default(SupportedBrowsersModel)));

            actual.ParamName.Should().Be("model");
        }
        
        [Test]
        public static async Task Post_SupportedBrowsers_InvalidModel_ReturnsViewWithModel()
        {
            var mockSupportedBrowsersModel = new Mock<SupportedBrowsersModel>().Object;
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.SupportedBrowsers(mockSupportedBrowsersModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockSupportedBrowsersModel);
        }

        [Test, AutoData]
        public static async Task Post_SupportedBrowsers_ValidModel_GetsClientApplicationFromService(
            SupportedBrowsersModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.SupportedBrowsers(model);
            
            mockService.Verify(x => x.GetClientApplication(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_SupportedBrowsers_NoClientApplicationFromService_ReturnsBadRequestResult(
            SupportedBrowsersModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(default(ClientApplication));
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.SupportedBrowsers(model)).As<BadRequestObjectResult>();
            
            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }
        
        [Test, AutoData]
        public static async Task Post_SupportedBrowsers_ValidModel_MapsModelToClientApplication(
            SupportedBrowsersModel model)
        {
            var mockMapper = new Mock<IMapper>();
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                mockMapper.Object, mockService.Object);

            await controller.SupportedBrowsers(model);

            mockMapper.Verify(x => x.Map(model, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_SupportedBrowsers_ValidModel_CallSaveClientApplicationOnService(
            SupportedBrowsersModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.SupportedBrowsers(model);

            mockService.Verify(x => x.SaveClientApplication(model.SolutionId, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_SupportedBrowsers_ValidModel_ReturnsRedirectResult(
            SupportedBrowsersModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            
            var actual = (await controller.SupportedBrowsers(model))
                .As<RedirectResult>();

            actual.Should().NotBeNull();
            actual.Url.Should().Be($"/marketing/supplier/solution/{model.SolutionId}/section/browser-based");
        }
        
        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_BrowserBasedMobileFirstApproach_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.MobileFirstApproach(id));
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_BrowserBasedPlugInsOrExtensions_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), 
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.PlugInsOrExtensions(id));
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_BrowserBasedConnectivityAndResolution_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), 
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.ConnectivityAndResolution(id));
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_BrowserBasedHardwareRequirements_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), 
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.HardwareRequirements(id));
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_BrowserBasedAdditionalInformation_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), 
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.AdditionalInformation(id));
        }
    }
}
