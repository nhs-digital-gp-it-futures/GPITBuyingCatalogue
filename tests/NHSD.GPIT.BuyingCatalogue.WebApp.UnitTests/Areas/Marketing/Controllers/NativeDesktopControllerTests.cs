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
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeDesktop;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class NativeDesktopControllerTests
    {        
        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(NativeDesktopController).Should()
                .BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
            typeof(NativeDesktopController).Should()
                .BeDecoratedWith<RouteAttribute>(x =>
                    x.Template == "marketing/supplier/solution/{id}/section/native-desktop");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                    _ = new NativeDesktopController(null, Mock.Of<IMapper>(), Mock.Of<ISolutionsService>()))
                .ParamName.Should().Be("logger");
        }

        [Test]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                    _ = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(), null,
                        Mock.Of<ISolutionsService>()))
                .ParamName.Should().Be("mapper");
        }

        [Test]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                    _ = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(), Mock.Of<IMapper>(),
                        null))
                .ParamName.Should().Be("solutionsService");
        }

        [Test]
        public static void Get_AdditionalInformation_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(NativeDesktopController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeDesktopController.AdditionalInformation)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(NativeDesktopController.AdditionalInformation).ToLowerCaseHyphenated());
        }

        [Test, AutoData]
        public static async Task Get_AdditionalInformation_ValidId_GetsSolutionFromService(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            await controller.AdditionalInformation(id);

            mockSolutionsService.Verify(s => s.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_AdditionalInformation_NullSolutionFromService_ReturnsBadRequestResult(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            var actual = (await controller.AdditionalInformation(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_AdditionalInformation_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            await controller.AdditionalInformation(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, AdditionalInformationModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_AdditionalInformation_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            var mockAdditionalInformationModel = new Mock<AdditionalInformationModel>().Object;
            mockMapper.Setup(m => m.Map<CatalogueItem, AdditionalInformationModel>(mockCatalogueItem))
                .Returns(mockAdditionalInformationModel);
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.AdditionalInformation(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockAdditionalInformationModel);
        }

        [Test]
        public static void Post_AdditionalInformation_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(NativeDesktopController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeDesktopController.AdditionalInformation)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(NativeDesktopController.AdditionalInformation).ToLowerCaseHyphenated());
        }

        [Test, AutoData]
        public static async Task Post_AdditionalInformation_ModelNotValid_DoesNotCallService(AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.AdditionalInformation(model);

            mockService.Verify(s => s.GetSolution(model.SolutionId), Times.Never);
        }

        [Test, AutoData]
        public static async Task Post_AdditionalInformation_ModelNotValid_ReturnsViewWithModel(AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.AdditionalInformation(model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(model);
        }

        [Test, AutoData]
        public static async Task Post_AdditionalInformation_ModelValid_GetsClientApplicationFromService(
            AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(new Mock<ClientApplication>().Object);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.AdditionalInformation(model);

            mockService.Verify(s => s.GetClientApplication(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_AdditionalInformation_NullClientApplicationFromService_ReturnsBadRequestResult(
            AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(default(ClientApplication));
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.AdditionalInformation(model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Test, AutoData]
        public static async Task Post_AdditionalInformation_ModelValid_MapsClientApplication(
            AdditionalInformationModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication.Object);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.AdditionalInformation(model);

            mockClientApplication.VerifySet(c =>
                c.NativeDesktopAdditionalInformation = model.AdditionalInformation);
        }

        [Test, AutoData]
        public static async Task Post_AdditionalInformation_ModelValid_CallsSaveClientApplication(
            AdditionalInformationModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.AdditionalInformation(model);

            mockService.Verify(s => s.SaveClientApplication(model.SolutionId, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_AdditionalInformation_ModelValid_ReturnsRedirectResult(
            AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(new Mock<ClientApplication>().Object);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.AdditionalInformation(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.NativeDesktop));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["id"].Should().Be(model.SolutionId);
        }

        [Test]
        public static void Get_Connectivity_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(NativeDesktopController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeDesktopController.Connectivity)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(NativeDesktopController.Connectivity).ToLowerCaseHyphenated());
        }

        [Test, AutoData]
        public static async Task Get_Connectivity_ValidId_GetsSolutionFromService(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            await controller.Connectivity(id);

            mockSolutionsService.Verify(s => s.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_Connectivity_NullSolutionFromService_ReturnsBadRequestResult(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            var actual = (await controller.Connectivity(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_Connectivity_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            await controller.Connectivity(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, ConnectivityModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_Connectivity_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            var mockConnectivityModel = new Mock<ConnectivityModel>().Object;
            mockMapper.Setup(m => m.Map<CatalogueItem, ConnectivityModel>(mockCatalogueItem))
                .Returns(mockConnectivityModel);
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.Connectivity(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockConnectivityModel);
        }

        [Test]
        public static void Post_Connectivity_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(NativeDesktopController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeDesktopController.Connectivity)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(NativeDesktopController.Connectivity).ToLowerCaseHyphenated());
        }

        [Test, AutoData]
        public static async Task Post_Connectivity_ModelNotValid_DoesNotCallService(ConnectivityModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.Connectivity(model);

            mockService.Verify(s => s.GetSolution(model.SolutionId), Times.Never);
        }

        [Test, AutoData]
        public static async Task Post_Connectivity_ModelNotValid_ReturnsViewWithModel(ConnectivityModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Connectivity(model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(model);
        }

        [Test, AutoData]
        public static async Task Post_Connectivity_ModelValid_GetsClientApplicationFromService(
            ConnectivityModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(new Mock<ClientApplication>().Object);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Connectivity(model);

            mockService.Verify(s => s.GetClientApplication(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_Connectivity_NullClientApplicationFromService_ReturnsBadRequestResult(
            ConnectivityModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(default(ClientApplication));
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.Connectivity(model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Test, AutoData]
        public static async Task Post_Connectivity_ModelValid_MapsClientApplication(
            ConnectivityModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication.Object);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Connectivity(model);

            mockClientApplication.VerifySet(c =>
                c.NativeDesktopMinimumConnectionSpeed = model.SelectedConnectionSpeed);
        }

        [Test, AutoData]
        public static async Task Post_Connectivity_ModelValid_CallsSaveClientApplication(
            ConnectivityModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Connectivity(model);

            mockService.Verify(s => s.SaveClientApplication(model.SolutionId, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_Connectivity_ModelValid_ReturnsRedirectResult(
            ConnectivityModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(new Mock<ClientApplication>().Object);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.Connectivity(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.NativeDesktop));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["id"].Should().Be(model.SolutionId);
        }

        [Test]
        public static void Get_HardwareRequirements_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(NativeDesktopController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeDesktopController.HardwareRequirements)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(NativeDesktopController.HardwareRequirements).ToLowerCaseHyphenated());
        }

        [Test, AutoData]
        public static async Task Get_HardwareRequirements_ValidId_GetsSolutionFromService(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            await controller.HardwareRequirements(id);

            mockSolutionsService.Verify(s => s.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_HardwareRequirements_NullSolutionFromService_ReturnsBadRequestResult(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            var actual = (await controller.HardwareRequirements(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_HardwareRequirements_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            await controller.HardwareRequirements(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, HardwareRequirementsModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_HardwareRequirements_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            var mockHardwareRequirementsModel = new Mock<HardwareRequirementsModel>().Object;
            mockMapper.Setup(m => m.Map<CatalogueItem, HardwareRequirementsModel>(mockCatalogueItem))
                .Returns(mockHardwareRequirementsModel);
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.HardwareRequirements(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockHardwareRequirementsModel);
        }

        [Test]
        public static void Post_HardwareRequirements_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(NativeDesktopController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeDesktopController.HardwareRequirements)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(NativeDesktopController.HardwareRequirements).ToLowerCaseHyphenated());
        }

        [Test, AutoData]
        public static async Task Post_HardwareRequirements_ModelNotValid_DoesNotCallService(HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.HardwareRequirements(model);

            mockService.Verify(s => s.GetSolution(model.SolutionId), Times.Never);
        }

        [Test, AutoData]
        public static async Task Post_HardwareRequirements_ModelNotValid_ReturnsViewWithModel(HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.HardwareRequirements(model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(model);
        }

        [Test, AutoData]
        public static async Task Post_HardwareRequirements_ModelValid_GetsClientApplicationFromService(
            HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(new Mock<ClientApplication>().Object);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.HardwareRequirements(model);

            mockService.Verify(s => s.GetClientApplication(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_HardwareRequirements_NullClientApplicationFromService_ReturnsBadRequestResult(
            HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(default(ClientApplication));
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.HardwareRequirements(model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Test, AutoData]
        public static async Task Post_HardwareRequirements_ModelValid_MapsClientApplication(
            HardwareRequirementsModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication.Object);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.HardwareRequirements(model);

            mockClientApplication.VerifySet(c =>
                c.NativeDesktopHardwareRequirements = model.Description);
        }

        [Test, AutoData]
        public static async Task Post_HardwareRequirements_ModelValid_CallsSaveClientApplication(
            HardwareRequirementsModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.HardwareRequirements(model);

            mockService.Verify(s => s.SaveClientApplication(model.SolutionId, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_HardwareRequirements_ModelValid_ReturnsRedirectResult(
            HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(new Mock<ClientApplication>().Object);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.HardwareRequirements(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.NativeDesktop));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["id"].Should().Be(model.SolutionId);
        }

        [Test]
        public static void Get_MemoryAndStorage_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(NativeDesktopController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeDesktopController.MemoryAndStorage)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(NativeDesktopController.MemoryAndStorage).ToLowerCaseHyphenated());
        }

        [Test, AutoData]
        public static async Task Get_MemoryAndStorage_ValidId_GetsSolutionFromService(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            await controller.MemoryAndStorage(id);

            mockSolutionsService.Verify(s => s.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_MemoryAndStorage_NullSolutionFromService_ReturnsBadRequestResult(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            var actual = (await controller.MemoryAndStorage(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_MemoryAndStorage_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            await controller.MemoryAndStorage(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, MemoryAndStorageModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_MemoryAndStorage_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            var mockMemoryAndStorageModel = new Mock<MemoryAndStorageModel>().Object;
            mockMapper.Setup(m => m.Map<CatalogueItem, MemoryAndStorageModel>(mockCatalogueItem))
                .Returns(mockMemoryAndStorageModel);
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.MemoryAndStorage(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockMemoryAndStorageModel);
        }

        [Test]
        public static void Post_MemoryAndStorage_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(NativeDesktopController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeDesktopController.MemoryAndStorage)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(NativeDesktopController.MemoryAndStorage).ToLowerCaseHyphenated());
        }

        [Test, AutoData]
        public static async Task Post_MemoryAndStorage_ModelNotValid_DoesNotCallService(MemoryAndStorageModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.MemoryAndStorage(model);

            mockService.Verify(s => s.GetSolution(model.SolutionId), Times.Never);
        }

        [Test, AutoData]
        public static async Task Post_MemoryAndStorage_ModelNotValid_ReturnsViewWithModel(MemoryAndStorageModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.MemoryAndStorage(model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(model);
        }

        [Test, AutoData]
        public static async Task Post_MemoryAndStorage_ModelValid_GetsClientApplicationFromService(
            MemoryAndStorageModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(new Mock<ClientApplication>().Object);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.MemoryAndStorage(model);

            mockService.Verify(s => s.GetClientApplication(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_MemoryAndStorage_NullClientApplicationFromService_ReturnsBadRequestResult(
            MemoryAndStorageModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(default(ClientApplication));
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.MemoryAndStorage(model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Test, AutoData]
        public static async Task Post_MemoryAndStorage_ModelValid_MapsClientApplication(
            MemoryAndStorageModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication.Object);
            var mockMapper = new Mock<IMapper>();
            var mockNativeDesktopMemoryAndStorage = new Mock<NativeDesktopMemoryAndStorage>().Object;
            mockMapper.Setup(m => m.Map<MemoryAndStorageModel, NativeDesktopMemoryAndStorage>(model))
                .Returns(mockNativeDesktopMemoryAndStorage);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                mockMapper.Object, mockService.Object);

            await controller.MemoryAndStorage(model);

            mockMapper.Verify(m => m.Map<MemoryAndStorageModel, NativeDesktopMemoryAndStorage>(model));
            mockClientApplication.VerifySet(c => c.NativeDesktopMemoryAndStorage = mockNativeDesktopMemoryAndStorage);
        }

        [Test, AutoData]
        public static async Task Post_MemoryAndStorage_ModelValid_CallsSaveClientApplication(
            MemoryAndStorageModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.MemoryAndStorage(model);

            mockService.Verify(s => s.SaveClientApplication(model.SolutionId, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_MemoryAndStorage_ModelValid_ReturnsRedirectResult(
            MemoryAndStorageModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(new Mock<ClientApplication>().Object);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.MemoryAndStorage(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.NativeDesktop));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["id"].Should().Be(model.SolutionId);
        }

        [Test]
        public static void Get_OperatingSystems_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(NativeDesktopController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeDesktopController.OperatingSystems)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(NativeDesktopController.OperatingSystems).ToLowerCaseHyphenated());
        }

        [Test, AutoData]
        public static async Task Get_OperatingSystems_ValidId_GetsSolutionFromService(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            await controller.OperatingSystems(id);

            mockSolutionsService.Verify(s => s.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_OperatingSystems_NullSolutionFromService_ReturnsBadRequestResult(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            var actual = (await controller.OperatingSystems(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_OperatingSystems_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            await controller.OperatingSystems(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, OperatingSystemsModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_OperatingSystems_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            var mockOperatingSystemsModel = new Mock<OperatingSystemsModel>().Object;
            mockMapper.Setup(m => m.Map<CatalogueItem, OperatingSystemsModel>(mockCatalogueItem))
                .Returns(mockOperatingSystemsModel);
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.OperatingSystems(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockOperatingSystemsModel);
        }

        [Test]
        public static void Post_OperatingSystems_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(NativeDesktopController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeDesktopController.OperatingSystems)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(NativeDesktopController.OperatingSystems).ToLowerCaseHyphenated());
        }

        [Test, AutoData]
        public static async Task Post_OperatingSystems_ModelNotValid_DoesNotCallService(OperatingSystemsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.OperatingSystems(model);

            mockService.Verify(s => s.GetSolution(model.SolutionId), Times.Never);
        }

        [Test, AutoData]
        public static async Task Post_OperatingSystems_ModelNotValid_ReturnsViewWithModel(OperatingSystemsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.OperatingSystems(model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(model);
        }

        [Test, AutoData]
        public static async Task Post_OperatingSystems_ModelValid_GetsClientApplicationFromService(
            OperatingSystemsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.OperatingSystems(model);

            mockService.Verify(s => s.GetClientApplication(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_OperatingSystems_NullClientApplicationFromService_ReturnsBadRequestResult(
            OperatingSystemsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(default(ClientApplication));
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.OperatingSystems(model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Test, AutoData]
        public static async Task Post_OperatingSystems_ModelValid_MapsClientApplication(
            OperatingSystemsModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication.Object);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.OperatingSystems(model);

            mockClientApplication.VerifySet(c =>
                c.NativeDesktopOperatingSystemsDescription = model.OperatingSystemsDescription);
        }

        [Test, AutoData]
        public static async Task Post_OperatingSystems_ModelValid_CallsSaveClientApplication(
            OperatingSystemsModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.OperatingSystems(model);

            mockService.Verify(s => s.SaveClientApplication(model.SolutionId, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_OperatingSystems_ModelValid_ReturnsRedirectResult(
            OperatingSystemsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(new Mock<ClientApplication>().Object);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.OperatingSystems(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.NativeDesktop));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["id"].Should().Be(model.SolutionId);
        }

        [Test]
        public static void Get_ThirdParty_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(NativeDesktopController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeDesktopController.ThirdParty)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(NativeDesktopController.ThirdParty).ToLowerCaseHyphenated());
        }

        [Test, AutoData]
        public static async Task Get_ThirdParty_ValidId_GetsSolutionFromService(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            await controller.ThirdParty(id);

            mockSolutionsService.Verify(s => s.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_ThirdParty_NullSolutionFromService_ReturnsBadRequestResult(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            var actual = (await controller.ThirdParty(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_ThirdParty_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            await controller.ThirdParty(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, ThirdPartyModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_ThirdParty_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            var mockThirdPartyModel = new Mock<ThirdPartyModel>().Object;
            mockMapper.Setup(m => m.Map<CatalogueItem, ThirdPartyModel>(mockCatalogueItem))
                .Returns(mockThirdPartyModel);
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.ThirdParty(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockThirdPartyModel);
        }

        [Test]
        public static void Post_ThirdParty_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(NativeDesktopController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeDesktopController.ThirdParty)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(NativeDesktopController.ThirdParty).ToLowerCaseHyphenated());
        }

        [Test, AutoData]
        public static async Task Post_ThirdParty_ModelNotValid_DoesNotCallService(ThirdPartyModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.ThirdParty(model);

            mockService.Verify(s => s.GetSolution(model.SolutionId), Times.Never);
        }

        [Test, AutoData]
        public static async Task Post_ThirdParty_ModelNotValid_ReturnsViewWithModel(ThirdPartyModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.ThirdParty(model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(model);
        }

        [Test, AutoData]
        public static async Task Post_ThirdParty_ModelValid_GetsClientApplicationFromService(
            ThirdPartyModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(new Mock<ClientApplication>().Object);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ThirdParty(model);

            mockService.Verify(s => s.GetClientApplication(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_ThirdParty_NullClientApplicationFromService_ReturnsBadRequestResult(
            ThirdPartyModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(default(ClientApplication));
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.ThirdParty(model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Test, AutoData]
        public static async Task Post_ThirdParty_ModelValid_MapsClientApplication(
            ThirdPartyModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>();
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication.Object);
            var mockMapper = new Mock<IMapper>();
            var mockNativeDesktopThirdParty = new Mock<NativeDesktopThirdParty>().Object;
            mockMapper.Setup(m => m.Map<ThirdPartyModel, NativeDesktopThirdParty>(model))
                .Returns(mockNativeDesktopThirdParty);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                mockMapper.Object, mockService.Object);

            await controller.ThirdParty(model);

            mockMapper.Verify(m => m.Map<ThirdPartyModel, NativeDesktopThirdParty>(model));
            mockClientApplication.VerifySet(c => c.NativeDesktopThirdParty = mockNativeDesktopThirdParty);
        }

        [Test, AutoData]
        public static async Task Post_ThirdParty_ModelValid_CallsSaveClientApplication(
            ThirdPartyModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ThirdParty(model);

            mockService.Verify(s => s.SaveClientApplication(model.SolutionId, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_ThirdParty_ModelValid_ReturnsRedirectResult(
            ThirdPartyModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(new Mock<ClientApplication>().Object);
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.ThirdParty(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.NativeDesktop));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["id"].Should().Be(model.SolutionId);
        }
    }
}
