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
        public static void Get_AdditionalInformation_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(BrowserBasedController)
                .GetMethods()
                .First(x => x.Name == nameof(BrowserBasedController.AdditionalInformation)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be("additional-information");
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_AdditionalInformation_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), 
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.AdditionalInformation(id));
        }

        [Test, AutoData]
        public static async Task Get_AdditionalInformation_ValidId_CallsGetSolutionOnService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), 
                Mock.Of<IMapper>(), mockService.Object);

            await controller.AdditionalInformation(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_AdditionalInformation_NullSolutionFromService_ReturnsBadRequestResponse(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.AdditionalInformation(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }
        
        [Test, AutoData]
        public static async Task Get_AdditionalInformation_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                mockMapper.Object, mockService.Object);

            await controller.AdditionalInformation(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, AdditionalInformationModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_AdditionalInformation_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockAdditionalInformationModel = new Mock<AdditionalInformationModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, AdditionalInformationModel>(mockCatalogueItem))
                .Returns(mockAdditionalInformationModel);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.AdditionalInformation(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockAdditionalInformationModel);
        }
        
        [Test]
        public static void Post_AdditionalInformation_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(BrowserBasedController)
                .GetMethods()
                .First(x => x.Name == nameof(BrowserBasedController.AdditionalInformation)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be("additional-information");
        }

        [Test]
        public static void Post_AdditionalInformation_NullModel_ThrowsException()
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            var actual = Assert.ThrowsAsync<ArgumentNullException>(() =>
                controller.AdditionalInformation(default(AdditionalInformationModel)));

            actual.ParamName.Should().Be("model");
        }
        
        [Test]
        public static async Task Post_AdditionalInformation_InvalidModel_ReturnsViewWithModel()
        {
            var mockAdditionalInformationModel = new Mock<AdditionalInformationModel>().Object;
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.AdditionalInformation(mockAdditionalInformationModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockAdditionalInformationModel);
        }

        [Test, AutoData]
        public static async Task Post_AdditionalInformation_ValidModel_GetsClientApplicationFromService(
            AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.AdditionalInformation(model);
            
            mockService.Verify(x => x.GetClientApplication(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_AdditionalInformation_NoClientApplicationFromService_ReturnsBadRequestResult(
            AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(default(ClientApplication));
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.AdditionalInformation(model)).As<BadRequestObjectResult>();
            
            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }
        
        [Test, AutoData]
        public static async Task Post_AdditionalInformation_ValidModel_MapsModelToClientApplication(
            AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>();
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication.Object);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.AdditionalInformation(model);

            mockClientApplication.VerifySet(x => x.AdditionalInformation = model.AdditionalInformation);
        }

        [Test, AutoData]
        public static async Task Post_AdditionalInformation_ValidModel_CallSaveClientApplicationOnService(
            AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.AdditionalInformation(model);

            mockService.Verify(x => x.SaveClientApplication(model.SolutionId, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_AdditionalInformation_ValidModel_ReturnsRedirectResult(
            AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            
            var actual = (await controller.AdditionalInformation(model)).As<RedirectResult>();

            actual.Should().NotBeNull();
            actual.Url.Should().Be($"/marketing/supplier/solution/{model.SolutionId}/section/browser-based");
        }
        
        [Test]
        public static void Get_ConnectivityAndResolution_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(BrowserBasedController)
                .GetMethods()
                .First(x => x.Name == nameof(BrowserBasedController.ConnectivityAndResolution)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be("connectivity-and-resolution");
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_ConnectivityAndResolution_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), 
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.ConnectivityAndResolution(id));
        }

        [Test, AutoData]
        public static async Task Get_ConnectivityAndResolution_ValidId_CallsGetSolutionOnService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), 
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ConnectivityAndResolution(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_ConnectivityAndResolution_NullSolutionFromService_ReturnsBadRequestResponse(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.ConnectivityAndResolution(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }
        
        [Test, AutoData]
        public static async Task Get_ConnectivityAndResolution_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                mockMapper.Object, mockService.Object);

            await controller.ConnectivityAndResolution(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, ConnectivityAndResolutionModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_ConnectivityAndResolution_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockConnectivityAndResolutionModel = new Mock<ConnectivityAndResolutionModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, ConnectivityAndResolutionModel>(mockCatalogueItem))
                .Returns(mockConnectivityAndResolutionModel);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.ConnectivityAndResolution(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockConnectivityAndResolutionModel);
        }
        
        [Test]
        public static void Post_ConnectivityAndResolution_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(BrowserBasedController)
                .GetMethods()
                .First(x => x.Name == nameof(BrowserBasedController.ConnectivityAndResolution)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be("connectivity-and-resolution");
        }

        [Test]
        public static void Post_ConnectivityAndResolution_NullModel_ThrowsException()
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            var actual = Assert.ThrowsAsync<ArgumentNullException>(() =>
                controller.ConnectivityAndResolution(default(ConnectivityAndResolutionModel)));

            actual.ParamName.Should().Be("model");
        }
        
        [Test]
        public static async Task Post_ConnectivityAndResolution_InvalidModel_ReturnsViewWithModel()
        {
            var mockConnectivityAndResolutionModel = new Mock<ConnectivityAndResolutionModel>().Object;
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.ConnectivityAndResolution(mockConnectivityAndResolutionModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockConnectivityAndResolutionModel);
        }

        [Test, AutoData]
        public static async Task Post_ConnectivityAndResolution_ValidModel_GetsClientApplicationFromService(
            ConnectivityAndResolutionModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ConnectivityAndResolution(model);
            
            mockService.Verify(x => x.GetClientApplication(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_ConnectivityAndResolution_NoClientApplicationFromService_ReturnsBadRequestResult(
            ConnectivityAndResolutionModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(default(ClientApplication));
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.ConnectivityAndResolution(model)).As<BadRequestObjectResult>();
            
            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }
        
        [Test, AutoData]
        public static async Task Post_ConnectivityAndResolution_ValidModel_MapsModelToClientApplication(
            ConnectivityAndResolutionModel model)
        {
            var mockMapper = new Mock<IMapper>();
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                mockMapper.Object, mockService.Object);

            await controller.ConnectivityAndResolution(model);

            mockMapper.Verify(x => x.Map(model, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_ConnectivityAndResolution_ValidModel_CallSaveClientApplicationOnService(
            ConnectivityAndResolutionModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ConnectivityAndResolution(model);

            mockService.Verify(x => x.SaveClientApplication(model.SolutionId, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_ConnectivityAndResolution_ValidModel_ReturnsRedirectResult(
            ConnectivityAndResolutionModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            
            var actual = (await controller.ConnectivityAndResolution(model)).As<RedirectResult>();

            actual.Should().NotBeNull();
            actual.Url.Should().Be($"/marketing/supplier/solution/{model.SolutionId}/section/browser-based");
        }
        
        [Test]
        public static void Get_HardwareRequirements_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(BrowserBasedController)
                .GetMethods()
                .First(x => x.Name == nameof(BrowserBasedController.HardwareRequirements)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be("hardware-requirements");
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_HardwareRequirements_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), 
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.HardwareRequirements(id));
        }

        [Test, AutoData]
        public static async Task Get_HardwareRequirements_ValidId_CallsGetSolutionOnService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), 
                Mock.Of<IMapper>(), mockService.Object);

            await controller.HardwareRequirements(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_HardwareRequirements_NullSolutionFromService_ReturnsBadRequestResponse(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.HardwareRequirements(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }
        
        [Test, AutoData]
        public static async Task Get_HardwareRequirements_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                mockMapper.Object, mockService.Object);

            await controller.HardwareRequirements(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, HardwareRequirementsModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_HardwareRequirements_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockHardwareRequirementsModel = new Mock<HardwareRequirementsModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, HardwareRequirementsModel>(mockCatalogueItem))
                .Returns(mockHardwareRequirementsModel);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.HardwareRequirements(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockHardwareRequirementsModel);
        }
        
        [Test]
        public static void Post_HardwareRequirements_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(BrowserBasedController)
                .GetMethods()
                .First(x => x.Name == nameof(BrowserBasedController.HardwareRequirements)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be("hardware-requirements");
        }

        [Test]
        public static void Post_HardwareRequirements_NullModel_ThrowsException()
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            var actual = Assert.ThrowsAsync<ArgumentNullException>(() =>
                controller.HardwareRequirements(default(HardwareRequirementsModel)));

            actual.ParamName.Should().Be("model");
        }
        
        [Test]
        public static async Task Post_HardwareRequirements_InvalidModel_ReturnsViewWithModel()
        {
            var mockHardwareRequirementsModel = new Mock<HardwareRequirementsModel>().Object;
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.HardwareRequirements(mockHardwareRequirementsModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockHardwareRequirementsModel);
        }

        [Test, AutoData]
        public static async Task Post_HardwareRequirements_ValidModel_GetsClientApplicationFromService(
            HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.HardwareRequirements(model);
            
            mockService.Verify(x => x.GetClientApplication(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_HardwareRequirements_NoClientApplicationFromService_ReturnsBadRequestResult(
            HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(default(ClientApplication));
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.HardwareRequirements(model)).As<BadRequestObjectResult>();
            
            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }
        
        [Test, AutoData]
        public static async Task Post_HardwareRequirements_ValidModel_MapsModelToClientApplication(
            HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>();
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication.Object);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.HardwareRequirements(model);

            mockClientApplication.VerifySet(x => x.HardwareRequirements = model.Description);
        }

        [Test, AutoData]
        public static async Task Post_HardwareRequirements_ValidModel_CallSaveClientApplicationOnService(
            HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.HardwareRequirements(model);

            mockService.Verify(x => x.SaveClientApplication(model.SolutionId, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_HardwareRequirements_ValidModel_ReturnsRedirectResult(
            HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            
            var actual = (await controller.HardwareRequirements(model)).As<RedirectResult>();

            actual.Should().NotBeNull();
            actual.Url.Should().Be($"/marketing/supplier/solution/{model.SolutionId}/section/browser-based");
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
            
            var actual = (await controller.SupportedBrowsers(model)).As<RedirectResult>();

            actual.Should().NotBeNull();
            actual.Url.Should().Be($"/marketing/supplier/solution/{model.SolutionId}/section/browser-based");
        }
        
        [Test]
        public static void Get_MobileFirstApproach_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(BrowserBasedController)
                .GetMethods()
                .First(x => x.Name == nameof(BrowserBasedController.MobileFirstApproach)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be("mobile-first-approach");
        }
        
        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_BrowserBasedMobileFirstApproach_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.MobileFirstApproach(id));
        }

        [Test, AutoData]
        public static async Task Get_MobileFirstApproach_ValidId_GetsSolutionFromService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.MobileFirstApproach(id);
            
            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_MobileFirstApproach_ServiceReturnsNull_ReturnsBadRequestResult(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.MobileFirstApproach(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }
        
        [Test, AutoData]
        public static async Task Get_MobileFirstApproach_ServiceResponseValid_MapsToModel(
            string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                mockMapper.Object, mockService.Object);

            await controller.MobileFirstApproach(id);
        
            mockMapper.Verify(x => x.Map<CatalogueItem, MobileFirstApproachModel>(mockCatalogueItem));
        }
        
        [Test, AutoData]
        public static async Task Get_MobileFirstApproach_ServiceResponseValid_ReturnsExpectedView(
            string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMobileFirstApproachModel = new Mock<MobileFirstApproachModel>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<CatalogueItem, MobileFirstApproachModel>(mockCatalogueItem))
                .Returns(mockMobileFirstApproachModel);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.MobileFirstApproach(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.Model.Should().Be(mockMobileFirstApproachModel);
        }

        [Test]
        public static void Post_MobileFirstApproach_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(BrowserBasedController)
                .GetMethods()
                .First(x => x.Name == nameof(BrowserBasedController.MobileFirstApproach)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be("mobile-first-approach");
        }
        
        [Test]
        public static void Post_MobileFirstApproach_NullModel_ThrowsException()
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            var actual = Assert.ThrowsAsync<ArgumentNullException>(() =>
                controller.MobileFirstApproach(default(MobileFirstApproachModel)));

            actual.ParamName.Should().Be("model");
        }
        
        [Test]
        public static async Task Post_MobileFirstApproach_InvalidModel_ReturnsViewWithModel()
        {
            var mockMobileFirstApproachModel = new Mock<MobileFirstApproachModel>().Object;
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.MobileFirstApproach(mockMobileFirstApproachModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockMobileFirstApproachModel);
        }

        [Test, AutoData]
        public static async Task Post_MobileFirstApproach_ValidModel_GetsClientApplicationFromService(
            MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.MobileFirstApproach(model);
            
            mockService.Verify(x => x.GetClientApplication(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_MobileFirstApproach_NoClientApplicationFromService_ReturnsBadRequestResult(
            MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(default(ClientApplication));
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.MobileFirstApproach(model)).As<BadRequestObjectResult>();
            
            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }
        
        [Test, AutoData]
        public static async Task Post_MobileFirstApproach_ValidModel_MapsModelToClientApplication(
            MobileFirstApproachModel model, bool? mobileFirstApproach)
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<string, bool?>(model.MobileFirstApproach))
                .Returns(mobileFirstApproach);
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>();
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication.Object);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                mockMapper.Object, mockService.Object);

            await controller.MobileFirstApproach(model);

            mockMapper.Verify(x => x.Map<string, bool?>(model.MobileFirstApproach));
            mockClientApplication.VerifySet(x => x.MobileFirstDesign = mobileFirstApproach);
        }

        [Test, AutoData]
        public static async Task Post_MobileFirstApproach_ValidModel_CallSaveClientApplicationOnService(
            MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.MobileFirstApproach(model);

            mockService.Verify(x => x.SaveClientApplication(model.SolutionId, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_MobileFirstApproach_ValidModel_ReturnsRedirectResult(
            MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            
            var actual = (await controller.MobileFirstApproach(model)).As<RedirectResult>();

            actual.Should().NotBeNull();
            actual.Url.Should().Be($"/marketing/supplier/solution/{model.SolutionId}/section/browser-based");
        }
        
        [Test]
        public static void Get_PlugInsOrExtensions_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(BrowserBasedController)
                .GetMethods()
                .First(x => x.Name == nameof(BrowserBasedController.PlugInsOrExtensions)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be("plug-ins-or-extensions");
        }
        
        [Test, AutoData]
        public static async Task Get_PlugInsOrExtensions_ValidId_GetsSolutionFromService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PlugInsOrExtensions(id);
            
            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_PlugInsOrExtensions_ServiceReturnsNull_ReturnsBadRequestResult(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PlugInsOrExtensions(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }
        
        [Test, AutoData]
        public static async Task Get_PlugInsOrExtensions_ServiceResponseValid_MapsToModel(
            string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                mockMapper.Object, mockService.Object);

            await controller.PlugInsOrExtensions(id);
        
            mockMapper.Verify(x => x.Map<CatalogueItem, PlugInsOrExtensionsModel>(mockCatalogueItem));
        }
        
        [Test, AutoData]
        public static async Task Get_PlugInsOrExtensions_ServiceResponseValid_ReturnsExpectedView(
            string id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockPlugInsOrExtensionsModel = new Mock<PlugInsOrExtensionsModel>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<CatalogueItem, PlugInsOrExtensionsModel>(mockCatalogueItem))
                .Returns(mockPlugInsOrExtensionsModel);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.PlugInsOrExtensions(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.Model.Should().Be(mockPlugInsOrExtensionsModel);
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_PlugInsOrExtensions_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), 
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.PlugInsOrExtensions(id));
        }
        
        [Test]
        public static void Post_PlugInsOrExtensions_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(BrowserBasedController)
                .GetMethods()
                .First(x => x.Name == nameof(BrowserBasedController.PlugInsOrExtensions)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be("plug-ins-or-extensions");
        }

        [Test]
        public static void Post_PlugInsOrExtensions_NullModel_ThrowsException()
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            var actual = Assert.ThrowsAsync<ArgumentNullException>(() =>
                controller.PlugInsOrExtensions(default(PlugInsOrExtensionsModel)));

            actual.ParamName.Should().Be("model");
        }
        
        [Test]
        public static async Task Post_PlugInsOrExtensions_InvalidModel_ReturnsViewWithModel()
        {
            var mockPlugInsOrExtensionsModel = new Mock<PlugInsOrExtensionsModel>().Object;
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.PlugInsOrExtensions(mockPlugInsOrExtensionsModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockPlugInsOrExtensionsModel);
        }

        [Test, AutoData]
        public static async Task Post_PlugInsOrExtensions_ValidModel_GetsClientApplicationFromService(
            PlugInsOrExtensionsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PlugInsOrExtensions(model);
            
            mockService.Verify(x => x.GetClientApplication(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_PlugInsOrExtensions_NoClientApplicationFromService_ReturnsBadRequestResult(
            PlugInsOrExtensionsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(default(ClientApplication));
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PlugInsOrExtensions(model)).As<BadRequestObjectResult>();
            
            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }
        
        [Test, AutoData]
        public static async Task Post_PlugInsOrExtensions_ValidModel_MapsModelToClientApplication(
            PlugInsOrExtensionsModel model, bool? PlugInsOrExtensions)
        {
            var mockMapper = new Mock<IMapper>();
            var mockPlugins = new Mock<Plugins>().Object;
            mockMapper.Setup(x => x.Map<PlugInsOrExtensionsModel, Plugins>(model))
                .Returns(mockPlugins);
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>();
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication.Object);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                mockMapper.Object, mockService.Object);

            await controller.PlugInsOrExtensions(model);

            mockMapper.Verify(x => x.Map<PlugInsOrExtensionsModel, Plugins>(model));
            mockClientApplication.VerifySet(x => x.Plugins = mockPlugins);
        }

        [Test, AutoData]
        public static async Task Post_PlugInsOrExtensions_ValidModel_CallSaveClientApplicationOnService(
            PlugInsOrExtensionsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PlugInsOrExtensions(model);

            mockService.Verify(x => x.SaveClientApplication(model.SolutionId, mockClientApplication));
        }

        [Test, AutoData]
        public static async Task Post_PlugInsOrExtensions_ValidModel_ReturnsRedirectResult(
            PlugInsOrExtensionsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(model.SolutionId))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            
            var actual = (await controller.PlugInsOrExtensions(model)).As<RedirectResult>();

            actual.Should().NotBeNull();
            actual.Url.Should().Be($"/marketing/supplier/solution/{model.SolutionId}/section/browser-based");
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
