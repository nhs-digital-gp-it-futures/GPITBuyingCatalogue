using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.ClientApplicationType;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ClientApplicationTypeControllerTests
    {
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
        [CommonAutoData]
        public static async Task Get_BrowserBased_ValidId_CallsGetSolutionOnService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.BrowserBased(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test]
        [CommonAutoData]
        public static async Task Get_BrowserBased_NullSolutionFromService_ReturnsBadRequestResponse(CatalogueItemId id)
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

        [Test]
        [CommonAutoData]
        public static async Task Get_BrowserBased_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
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

        [Test]
        [CommonAutoData]
        public static async Task Get_BrowserBased_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
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
        [CommonAutoData]
        public static async Task Get_NativeDesktop_ValidId_CallsGetSolutionOnService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.NativeDesktop(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test]
        [CommonAutoData]
        public static async Task Get_NativeDesktop_NullSolutionFromService_ReturnsBadRequestResponse(CatalogueItemId id)
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

        [Test]
        [CommonAutoData]
        public static async Task Get_NativeDesktop_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
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

        [Test]
        [CommonAutoData]
        public static async Task Get_NativeDesktop_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
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
        [CommonAutoData]
        public static async Task Get_NativeMobile_ValidId_CallsGetSolutionOnService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.NativeMobile(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test]
        [CommonAutoData]
        public static async Task Get_NativeMobile_NullSolutionFromService_ReturnsBadRequestResponse(CatalogueItemId id)
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

        [Test]
        [CommonAutoData]
        public static async Task Get_NativeMobile_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
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

        [Test]
        [CommonAutoData]
        public static async Task Get_NativeMobile_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
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
        [CommonAutoData]
        public static async Task Get_ClientApplicationTypes_ValidId_CallsGetSolutionOnService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ClientApplicationTypes(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test]
        [CommonAutoData]
        public static async Task Get_ClientApplicationTypes_NullSolutionFromService_ReturnsBadRequestResponse(CatalogueItemId id)
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

        [Test]
        [CommonAutoData]
        public static async Task Get_ClientApplicationTypes_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
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

        [Test]
        [CommonAutoData]
        public static async Task Get_ClientApplicationTypes_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
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
        [CommonAutoData]
        public static async Task Post_ClientApplicationTypes_InvalidModel_ReturnsViewWithModel([Frozen] CatalogueItemId id)
        {
            var mockClientApplicationTypesModel = new Mock<ClientApplicationTypesModel>().Object;
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.ClientApplicationTypes(id, mockClientApplicationTypesModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockClientApplicationTypesModel);
        }

        [Test]
        [CommonAutoData]
        public static async Task Post_ClientApplicationTypes_ValidModel_GetsClientApplicationFromService(
            [Frozen] CatalogueItemId id,
            ClientApplicationTypesModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ClientApplicationTypes(id, model);

            mockService.Verify(s => s.GetClientApplication(id));
        }

        [Test]
        [CommonAutoData]
        public static async Task Post_ClientApplicationTypes_NoClientApplicationFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            ClientApplicationTypesModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(id)).ReturnsAsync(default(ClientApplication));
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.ClientApplicationTypes(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Test]
        [CommonAutoData]
        public static async Task Post_ClientApplicationTypes_ValidModel_MapsModelToClientApplication(
            [Frozen] CatalogueItemId id,
            ClientApplicationTypesModel model)
        {
            var mockMapper = new Mock<IMapper>();
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(s => s.GetClientApplication(id)).ReturnsAsync(mockClientApplication);
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                mockMapper.Object, mockService.Object);

            await controller.ClientApplicationTypes(id, model);

            mockMapper.Verify(x => x.Map(model, mockClientApplication));
        }

        [Test]
        [CommonAutoData]
        public static async Task Post_ClientApplicationTypes_ValidModel_CallSaveClientApplicationOnService(
            [Frozen] CatalogueItemId id,
            ClientApplicationTypesModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(s => s.GetClientApplication(id)).ReturnsAsync(mockClientApplication);
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ClientApplicationTypes(id, model);

            mockService.Verify(x => x.SaveClientApplication(id, mockClientApplication));
        }

        [Test]
        [CommonAutoData]
        public static async Task Post_ClientApplicationTypes_ValidModel_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            ClientApplicationTypesModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(s => s.GetClientApplication(id)).ReturnsAsync(mockClientApplication);
            var controller = new ClientApplicationTypeController(Mock.Of<ILogWrapper<ClientApplicationTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.ClientApplicationTypes(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be("Solution");
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }
    }
}
