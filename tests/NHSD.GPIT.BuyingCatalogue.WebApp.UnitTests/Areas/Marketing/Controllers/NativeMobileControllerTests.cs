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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class NativeMobileControllerTests
    {
        private static string[] InvalidStrings = { null, string.Empty, "    " };

        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(NativeMobileController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new NativeMobileController(null, Mock.Of<IMapper>(), Mock.Of<ISolutionsService>()))
                .ParamName.Should().Be("logger");
        }
        
        [Test]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                    _ = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(), null,
                        Mock.Of<ISolutionsService>()))
                .ParamName.Should().Be("mapper");
        }

        [Test]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(), Mock.Of<IMapper>(),
                    null))
                .ParamName.Should().Be("solutionsService");
        }
        
        [Test]
        public static void Get_AdditionalInformation_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(NativeMobileController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeMobileController.AdditionalInformation)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(NativeMobileController.AdditionalInformation).ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_AdditionalInformation_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.AdditionalInformation(id))
                .Message.Should().Be("id");
        }

        [Test, AutoData]
        public static async Task Get_AdditionalInformation_ValidId_GetsSolutionFromService(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.AdditionalInformation(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockAdditionalInformationModel);
        }
        
        [Test]
        public static void Post_AdditionalInformation_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(NativeMobileController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeMobileController.AdditionalInformation)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(NativeMobileController.AdditionalInformation).ToLowerCaseHyphenated());
        }

        [Test]
        public static async Task Post_AdditionalInformation_ModelNull_ThrowsException()
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.AdditionalInformation(default(AdditionalInformationModel)))
                .ParamName.Should().Be("model");
        }

        [Test, AutoData]
        public static async Task Post_AdditionalInformation_ModelNotValid_DoesNotCallService(AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.AdditionalInformation(model);

            mockService.Verify(s => s.GetSolution(model.SolutionId), Times.Never);
        }

        [Test, AutoData]
        public static async Task Post_AdditionalInformation_ModelNotValid_ReturnsViewWithModel(AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.AdditionalInformation(model);

            mockClientApplication.VerifySet(c =>
                c.NativeMobileAdditionalInformation = model.AdditionalInformation);
        }

        [Test, AutoData]
        public static async Task Post_AdditionalInformation_ModelValid_CallsSaveClientApplication(
            AdditionalInformationModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.AdditionalInformation(model)).As<RedirectResult>();

            actual.Should().NotBeNull();
            actual.Url.Should().Be(GetRedirectUrl(model.SolutionId));
        }
        
        [Test]
        public static void Get_Connectivity_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(NativeMobileController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeMobileController.Connectivity)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(NativeMobileController.Connectivity).ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_Connectivity_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.Connectivity(id))
                .Message.Should().Be("id");
        }

        [Test, AutoData]
        public static async Task Get_Connectivity_ValidId_GetsSolutionFromService(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.Connectivity(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockConnectivityModel);
        }

        [Test]
        public static void Post_Connectivity_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(NativeMobileController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeMobileController.Connectivity)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(NativeMobileController.Connectivity).ToLowerCaseHyphenated());
        }

        [Test]
        public static async Task Post_Connectivity_ModelNull_ThrowsException()
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.Connectivity(default(ConnectivityModel)))
                .ParamName.Should().Be("model");
        }

        [Test, AutoData]
        public static async Task Post_Connectivity_ModelNotValid_DoesNotCallService(ConnectivityModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.Connectivity(model);

            mockService.Verify(s => s.GetSolution(model.SolutionId), Times.Never);
        }

        [Test, AutoData]
        public static async Task Post_Connectivity_ModelNotValid_ReturnsViewWithModel(ConnectivityModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var mockMapper = new Mock<IMapper>();
            var mockMobileConnectionDetails = new Mock<MobileConnectionDetails>().Object;
            mockMapper.Setup(m => m.Map<ConnectivityModel, MobileConnectionDetails>(model))
                .Returns(mockMobileConnectionDetails);
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                mockMapper.Object, mockService.Object);

            await controller.Connectivity(model);

            mockMapper.Verify(m => m.Map<ConnectivityModel, MobileConnectionDetails>(model));
            mockClientApplication.VerifySet(c => c.MobileConnectionDetails = mockMobileConnectionDetails);
        }

        [Test, AutoData]
        public static async Task Post_Connectivity_ModelValid_CallsSaveClientApplication(
            ConnectivityModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.Connectivity(model)).As<RedirectResult>();

            actual.Should().NotBeNull();
            actual.Url.Should().Be(GetRedirectUrl(model.SolutionId));
        }
        
        [Test]
        public static void Get_HardwareRequirements_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(NativeMobileController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeMobileController.HardwareRequirements)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(NativeMobileController.HardwareRequirements).ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_HardwareRequirements_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.HardwareRequirements(id))
                .Message.Should().Be("id");
        }

        [Test, AutoData]
        public static async Task Get_HardwareRequirements_ValidId_GetsSolutionFromService(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.HardwareRequirements(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockHardwareRequirementsModel);
        }
        
        [Test]
        public static void Post_HardwareRequirements_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(NativeMobileController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeMobileController.HardwareRequirements)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(NativeMobileController.HardwareRequirements).ToLowerCaseHyphenated());
        }

        [Test]
        public static async Task Post_HardwareRequirements_ModelNull_ThrowsException()
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.HardwareRequirements(default(HardwareRequirementsModel)))
                .ParamName.Should().Be("model");
        }

        [Test, AutoData]
        public static async Task Post_HardwareRequirements_ModelNotValid_DoesNotCallService(HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.HardwareRequirements(model);

            mockService.Verify(s => s.GetSolution(model.SolutionId), Times.Never);
        }

        [Test, AutoData]
        public static async Task Post_HardwareRequirements_ModelNotValid_ReturnsViewWithModel(HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.HardwareRequirements(model);

            mockClientApplication.VerifySet(c =>
                c.NativeMobileHardwareRequirements = model.Description);
        }

        [Test, AutoData]
        public static async Task Post_HardwareRequirements_ModelValid_CallsSaveClientApplication(
            HardwareRequirementsModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.HardwareRequirements(model)).As<RedirectResult>();

            actual.Should().NotBeNull();
            actual.Url.Should().Be(GetRedirectUrl(model.SolutionId));
        }
        
        [Test]
        public static void Get_MemoryAndStorage_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(NativeMobileController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeMobileController.MemoryAndStorage)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(NativeMobileController.MemoryAndStorage).ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_MemoryAndStorage_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.MemoryAndStorage(id))
                .Message.Should().Be("id");
        }

        [Test, AutoData]
        public static async Task Get_MemoryAndStorage_ValidId_GetsSolutionFromService(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.MemoryAndStorage(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockMemoryAndStorageModel);
        }
        
        [Test]
        public static void Post_MemoryAndStorage_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(NativeMobileController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeMobileController.MemoryAndStorage)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(NativeMobileController.MemoryAndStorage).ToLowerCaseHyphenated());
        }

        [Test]
        public static async Task Post_MemoryAndStorage_ModelNull_ThrowsException()
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.MemoryAndStorage(default(MemoryAndStorageModel)))
                .ParamName.Should().Be("model");
        }

        [Test, AutoData]
        public static async Task Post_MemoryAndStorage_ModelNotValid_DoesNotCallService(MemoryAndStorageModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.MemoryAndStorage(model);

            mockService.Verify(s => s.GetSolution(model.SolutionId), Times.Never);
        }

        [Test, AutoData]
        public static async Task Post_MemoryAndStorage_ModelNotValid_ReturnsViewWithModel(MemoryAndStorageModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var mockMobileMemoryAndStorage = new Mock<MobileMemoryAndStorage>().Object;
            mockMapper.Setup(m => m.Map<MemoryAndStorageModel, MobileMemoryAndStorage>(model))
                .Returns(mockMobileMemoryAndStorage);
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                mockMapper.Object, mockService.Object);

            await controller.MemoryAndStorage(model);

            mockMapper.Verify(m => m.Map<MemoryAndStorageModel, MobileMemoryAndStorage>(model));
            mockClientApplication.VerifySet(c => c.MobileMemoryAndStorage = mockMobileMemoryAndStorage);
        }

        [Test, AutoData]
        public static async Task Post_MemoryAndStorage_ModelValid_CallsSaveClientApplication(
            MemoryAndStorageModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.MemoryAndStorage(model)).As<RedirectResult>();

            actual.Should().NotBeNull();
            actual.Url.Should().Be(GetRedirectUrl(model.SolutionId));
        }
        
        [Test]
        public static void Get_MobileFirstApproach_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(NativeMobileController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeMobileController.MobileFirstApproach)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(NativeMobileController.MobileFirstApproach).ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_MobileFirstApproach_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.MobileFirstApproach(id))
                .Message.Should().Be("id");
        }

        [Test, AutoData]
        public static async Task Get_MobileFirstApproach_ValidId_GetsSolutionFromService(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            await controller.MobileFirstApproach(id);

            mockSolutionsService.Verify(s => s.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_MobileFirstApproach_NullSolutionFromService_ReturnsBadRequestResult(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            var actual = (await controller.MobileFirstApproach(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_MobileFirstApproach_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            await controller.MobileFirstApproach(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, MobileFirstApproachModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_MobileFirstApproach_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            var mockMobileFirstApproachModel = new Mock<MobileFirstApproachModel>().Object;
            mockMapper.Setup(m => m.Map<CatalogueItem, MobileFirstApproachModel>(mockCatalogueItem))
                .Returns(mockMobileFirstApproachModel);
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.MobileFirstApproach(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockMobileFirstApproachModel);
        }
        
        [Test]
        public static void Post_MobileFirstApproach_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(NativeMobileController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeMobileController.MobileFirstApproach)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(NativeMobileController.MobileFirstApproach).ToLowerCaseHyphenated());
        }

        [Test]
        public static void Post_MobileFirstApproach_ModelNull_ThrowsException()
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.MobileFirstApproach(default(MobileFirstApproachModel)))
                .ParamName.Should().Be("model");
        }

        [Test, AutoData]
        public static async Task Post_MobileFirstApproach_ModelNotValid_DoesNotCallService(MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.MobileFirstApproach(model);

            mockService.Verify(s => s.GetSolution(model.SolutionId), Times.Never);
        }

        [Test, AutoData]
        public static async Task Post_MobileFirstApproach_ModelNotValid_ReturnsViewWithModel(MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.MobileFirstApproach(model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(model);
        }

        [Test, AutoData]
        public static async Task Post_MobileFirstApproach_ModelValid_GetsClientApplicationFromService(
            MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.MobileFirstApproach(model);

            mockService.Verify(s => s.GetClientApplication(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_MobileFirstApproach_NullClientApplicationFromService_ReturnsBadRequestResult(
            MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(default(ClientApplication));
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.MobileFirstApproach(model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Test, AutoData]
        public static async Task Post_MobileFirstApproach_ModelValid_MapsClientApplication(
            string solutionId,
            bool? mobileFirstDesign)
        {
            var mockModel = new Mock<MobileFirstApproachModel>
            {
                CallBase = true,
            };
            mockModel.Object.SolutionId = solutionId;
            mockModel.Setup(m => m.MobileFirstDesign())
                .Returns(mobileFirstDesign);
            var mockClientApplication = new Mock<ClientApplication>();
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(solutionId))
                .ReturnsAsync(mockClientApplication.Object);
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.MobileFirstApproach(mockModel.Object);

            mockModel.Verify(m => m.MobileFirstDesign());
            mockClientApplication.VerifySet(c => c.NativeMobileFirstDesign = mobileFirstDesign);
        }

        [Test, AutoData]
        public static async Task Post_MobileFirstApproach_ModelValid_CallsSaveClientApplication(
            MobileFirstApproachModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.MobileFirstApproach(model);

            mockService.Verify(s => s.SaveClientApplication(model.SolutionId, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_MobileFirstApproach_ModelValid_ReturnsRedirectResult(
            MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(new Mock<ClientApplication>().Object);
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.MobileFirstApproach(model)).As<RedirectResult>();

            actual.Should().NotBeNull();
            actual.Url.Should().Be(GetRedirectUrl(model.SolutionId));
        }
        
        [Test]
        public static void Get_OperatingSystems_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(NativeMobileController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeMobileController.OperatingSystems)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(NativeMobileController.OperatingSystems).ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_OperatingSystems_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.OperatingSystems(id))
                .Message.Should().Be("id");
        }

        [Test, AutoData]
        public static async Task Get_OperatingSystems_ValidId_GetsSolutionFromService(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.OperatingSystems(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockOperatingSystemsModel);
        }
        
        [Test]
        public static void Post_OperatingSystems_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(NativeMobileController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeMobileController.OperatingSystems)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(NativeMobileController.OperatingSystems).ToLowerCaseHyphenated());
        }

        [Test]
        public static void Post_OperatingSystems_ModelNull_ThrowsException()
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.OperatingSystems(default(OperatingSystemsModel)))
                .ParamName.Should().Be("model");
        }

        [Test, AutoData]
        public static async Task Post_OperatingSystems_ModelNotValid_DoesNotCallService(OperatingSystemsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.OperatingSystems(model);

            mockService.Verify(s => s.GetSolution(model.SolutionId), Times.Never);
        }

        [Test, AutoData]
        public static async Task Post_OperatingSystems_ModelNotValid_ReturnsViewWithModel(OperatingSystemsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.OperatingSystems(model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Test, AutoData]
        public static async Task Post_OperatingSystems_ModelValid_MapsClientApplication(
            OperatingSystemsModel model)
        {
            var mockMobileOperatingSystems = new Mock<MobileOperatingSystems>().Object;
            var mockClientApplication = new Mock<ClientApplication>();
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<OperatingSystemsModel, MobileOperatingSystems>(model))
                .Returns(mockMobileOperatingSystems);
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication.Object);
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                mockMapper.Object, mockService.Object);

            await controller.OperatingSystems(model);

            mockClientApplication.VerifySet(c => c.MobileOperatingSystems = mockMobileOperatingSystems);
        }

        [Test, AutoData]
        public static async Task Post_OperatingSystems_ModelValid_CallsSaveClientApplication(
            OperatingSystemsModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.OperatingSystems(model)).As<RedirectResult>();

            actual.Should().NotBeNull();
            actual.Url.Should().Be(GetRedirectUrl(model.SolutionId));
        }

        [Test]
        public static void Get_ThirdParty_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(NativeMobileController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeMobileController.ThirdParty)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(NativeMobileController.ThirdParty).ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_ThirdParty_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.ThirdParty(id))
                .Message.Should().Be("id");
        }

        [Test, AutoData]
        public static async Task Get_ThirdParty_ValidId_GetsSolutionFromService(string id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.ThirdParty(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockThirdPartyModel);
        }
        
        [Test]
        public static void Post_ThirdParty_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(NativeMobileController)
                .GetMethods()
                .First(x => x.Name == nameof(NativeMobileController.ThirdParty)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(NativeMobileController.ThirdParty).ToLowerCaseHyphenated());
        }

        [Test]
        public static async Task Post_ThirdParty_ModelNull_ThrowsException()
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.ThirdParty(default(ThirdPartyModel)))
                .ParamName.Should().Be("model");
        }

        [Test, AutoData]
        public static async Task Post_ThirdParty_ModelNotValid_DoesNotCallService(ThirdPartyModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.ThirdParty(model);

            mockService.Verify(s => s.GetSolution(model.SolutionId), Times.Never);
        }

        [Test, AutoData]
        public static async Task Post_ThirdParty_ModelNotValid_ReturnsViewWithModel(ThirdPartyModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var mockMobileThirdParty = new Mock<MobileThirdParty>().Object;
            mockMapper.Setup(m => m.Map<ThirdPartyModel, MobileThirdParty>(model))
                .Returns(mockMobileThirdParty);
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                mockMapper.Object, mockService.Object);

            await controller.ThirdParty(model);

            mockMapper.Verify(m => m.Map<ThirdPartyModel, MobileThirdParty>(model));
            mockClientApplication.VerifySet(c => c.MobileThirdParty = mockMobileThirdParty);
        }

        [Test, AutoData]
        public static async Task Post_ThirdParty_ModelValid_CallsSaveClientApplication(
            ThirdPartyModel model)
        {
            var mockClientApplication = new Mock<ClientApplication>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
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
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.ThirdParty(model)).As<RedirectResult>();

            actual.Should().NotBeNull();
            actual.Url.Should().Be(GetRedirectUrl(model.SolutionId));
        }
        
        private static string GetRedirectUrl(string solutionId) =>
            $"/marketing/supplier/solution/{solutionId}/section/native-mobile";
    }
}
