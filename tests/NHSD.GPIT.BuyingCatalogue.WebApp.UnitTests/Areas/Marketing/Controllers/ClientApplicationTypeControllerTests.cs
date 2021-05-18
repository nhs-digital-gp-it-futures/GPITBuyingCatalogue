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
using NHSD.GPIT.BuyingCatalogue.Test.Framework;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ClientApplicationTypeControllerTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };

        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(ClientApplicationTypeController).Should()
                .BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
            typeof(ClientApplicationTypeController).Should()
                .BeDecoratedWith<RouteAttribute>(x => x.Template == "marketing/supplier/solution/{id}/section");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new ClientApplicationTypeController(null, Mock.Of<IMapper>(), Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(), null,
                    Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                    Mock.Of<IMapper>(), null));
        }

        [Test]
        public static void Get_BrowserBased_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(ClientApplicationTypeController)
                .GetMethods()
                .First(x => x.Name == nameof(ClientApplicationTypeController.BrowserBased)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(ClientApplicationTypeController.BrowserBased).ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_BrowserBased_InvalidId_ThrowsException(string id)
        {
            var controller = new ClientApplicationTypeController(
                Mock.Of<ILogWrapper<ClientApplicationTypeController>>(), Mock.Of<IMapper>(),
                Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.BrowserBased(id));
        }

        [Test, AutoData]
        public static async Task Get_BrowserBased_ValidId_CallsGetSolutionOnService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.BrowserBased(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_BrowserBased_NullSolutionFromService_ReturnsBadRequestResponse(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new ClientApplicationTypeController(
                Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.BrowserBased(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_BrowserBased_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                mockMapper.Object, mockService.Object);

            await controller.BrowserBased(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, BrowserBasedModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_BrowserBased_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockBrowserBasedModel = new Mock<BrowserBasedModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, BrowserBasedModel>(mockCatalogueItem))
                .Returns(mockBrowserBasedModel);
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.BrowserBased(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockBrowserBasedModel);
        }

        [Test]
        public static void Get_NativeDesktop_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(ClientApplicationTypeController)
                .GetMethods()
                .First(x => x.Name == nameof(ClientApplicationTypeController.NativeDesktop)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(ClientApplicationTypeController.NativeDesktop).ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_NativeDesktop_InvalidId_ThrowsException(string id)
        {
            var controller = new ClientApplicationTypeController(
                Mock.Of<ILogWrapper<ClientApplicationTypeController>>(), Mock.Of<IMapper>(),
                Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.NativeDesktop(id));
        }

        [Test, AutoData]
        public static async Task Get_NativeDesktop_ValidId_CallsGetSolutionOnService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.NativeDesktop(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_NativeDesktop_NullSolutionFromService_ReturnsBadRequestResponse(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new ClientApplicationTypeController(
                Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.NativeDesktop(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_NativeDesktop_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                mockMapper.Object, mockService.Object);

            await controller.NativeDesktop(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, NativeDesktopModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_NativeDesktop_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockNativeDesktopModel = new Mock<NativeDesktopModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, NativeDesktopModel>(mockCatalogueItem))
                .Returns(mockNativeDesktopModel);
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.NativeDesktop(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockNativeDesktopModel);
        }

        [Test]
        public static void Get_NativeMobile_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(ClientApplicationTypeController)
                .GetMethods()
                .First(x => x.Name == nameof(ClientApplicationTypeController.NativeMobile)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(ClientApplicationTypeController.NativeMobile).ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_NativeMobile_InvalidId_ThrowsException(string id)
        {
            var controller = new ClientApplicationTypeController(
                Mock.Of<ILogWrapper<ClientApplicationTypeController>>(), Mock.Of<IMapper>(),
                Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.NativeMobile(id));
        }

        [Test, AutoData]
        public static async Task Get_NativeMobile_ValidId_CallsGetSolutionOnService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.NativeMobile(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_NativeMobile_NullSolutionFromService_ReturnsBadRequestResponse(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new ClientApplicationTypeController(
                Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.NativeMobile(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_NativeMobile_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                mockMapper.Object, mockService.Object);

            await controller.NativeMobile(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, NativeMobileModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_NativeMobile_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockNativeMobileModel = new Mock<NativeMobileModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, NativeMobileModel>(mockCatalogueItem))
                .Returns(mockNativeMobileModel);
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.NativeMobile(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockNativeMobileModel);
        }

        [Test]
        public static void Get_ClientApplicationTypes_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(ClientApplicationTypeController)
                .GetMethods()
                .First(x => x.Name == nameof(ClientApplicationTypeController.ClientApplicationTypes)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(ClientApplicationTypeController.ClientApplicationTypes).ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_ClientApplicationTypes_InvalidId_ThrowsException(string id)
        {
            var controller = new ClientApplicationTypeController(
                Mock.Of<ILogWrapper<ClientApplicationTypeController>>(), Mock.Of<IMapper>(),
                Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.ClientApplicationTypes(id));
        }

        [Test, AutoData]
        public static async Task Get_ClientApplicationTypes_ValidId_CallsGetSolutionOnService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ClientApplicationTypes(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_ClientApplicationTypes_NullSolutionFromService_ReturnsBadRequestResponse(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.ClientApplicationTypes(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_ClientApplicationTypes_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                mockMapper.Object, mockService.Object);

            await controller.ClientApplicationTypes(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, ClientApplicationTypesModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_ClientApplicationTypes_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockClientApplicationTypesModel = new Mock<ClientApplicationTypesModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, ClientApplicationTypesModel>(mockCatalogueItem))
                .Returns(mockClientApplicationTypesModel);
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.ClientApplicationTypes(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockClientApplicationTypesModel);
        }

        [Test]
        public static void Post_ClientApplicationTypes_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(ClientApplicationTypeController)
                .GetMethods()
                .First(x => x.Name == nameof(ClientApplicationTypeController.ClientApplicationTypes)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(ClientApplicationTypeController.ClientApplicationTypes).ToLowerCaseHyphenated());
        }

        [Test]
        public static void Post_ClientApplicationTypes_NullModel_ThrowsException()
        {
            var controller = new ClientApplicationTypeController(
                Mock.Of<ILogWrapper<ClientApplicationTypeController>>(), Mock.Of<IMapper>(),
                Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() =>
                controller.ClientApplicationTypes((ClientApplicationTypesModel)null));
        }

        [Test]
        public static async Task Post_ClientApplicationTypes_InvalidModel_ReturnsViewWithModel()
        {
            var mockClientApplicationTypesModel = new Mock<ClientApplicationTypesModel>().Object;
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.ClientApplicationTypes(mockClientApplicationTypesModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockClientApplicationTypesModel);
        }

        [Test, AutoData]
        public static async Task Post_ClientApplicationTypes_ValidModel_GetsClientApplicationFromService(
            ClientApplicationTypesModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ClientApplicationTypes(model);

            mockService.Verify(x => x.GetClientApplication(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_ClientApplicationTypes_NoClientApplicationFromService_ReturnsBadRequestResult(
            ClientApplicationTypesModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(default(ClientApplication));
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.ClientApplicationTypes(model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Test, AutoData]
        public static async Task Post_ClientApplicationTypes_ValidModel_MapsModelToClientApplication(
            ClientApplicationTypesModel model)
        {
            var mockMapper = new Mock<IMapper>();
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                mockMapper.Object, mockService.Object);

            await controller.ClientApplicationTypes(model);

            mockMapper.Verify(x => x.Map(model, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_ClientApplicationTypes_ValidModel_CallSaveClientApplicationOnService(
            ClientApplicationTypesModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ClientApplicationTypes(model);

            mockService.Verify(x => x.SaveClientApplication(model.SolutionId, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_ClientApplicationTypes_ValidModel_ReturnsRedirectResult(
            ClientApplicationTypesModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.ClientApplicationTypes(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be("Solution");
            actual.RouteValues["id"].Should().Be(model.SolutionId);
        }
    }
}
