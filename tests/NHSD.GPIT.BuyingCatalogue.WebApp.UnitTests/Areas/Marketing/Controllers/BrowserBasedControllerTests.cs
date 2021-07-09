using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    public static class BrowserBasedControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(BrowserBasedController).Should()
                .BeDecoratedWith<AuthorizeAttribute>(p => p.Policy == "AdminOnly");

            typeof(BrowserBasedController).Should()
                .BeDecoratedWith<AreaAttribute>(r => r.RouteValue == "Marketing");
        }

        [Fact]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new BrowserBasedController( null,
                    Mock.Of<ISolutionsService>()));
        }

        [Fact]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new BrowserBasedController( Mock.Of<IMapper>(),
                    null));
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_ValidId_CallsGetSolutionOnService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.AdditionalInformation(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_NullSolutionFromService_ReturnsBadRequestResponse(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.AdditionalInformation(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new BrowserBasedController(
                mockMapper.Object, mockService.Object);

            await controller.AdditionalInformation(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, AdditionalInformationModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockAdditionalInformationModel = new Mock<AdditionalInformationModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, AdditionalInformationModel>(mockCatalogueItem))
                .Returns(mockAdditionalInformationModel);
            var controller = new BrowserBasedController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.AdditionalInformation(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockAdditionalInformationModel);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_InvalidModel_ReturnsViewWithModel([Frozen] CatalogueItemId id)
        {
            var mockAdditionalInformationModel = new Mock<AdditionalInformationModel>().Object;
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.AdditionalInformation(id, mockAdditionalInformationModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockAdditionalInformationModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_ValidModel_GetsClientApplicationFromService(
            [Frozen] CatalogueItemId id,
            AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.AdditionalInformation(id, model);

            mockService.Verify(s => s.GetClientApplication(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_NoClientApplicationFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(default(ClientApplication));
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.AdditionalInformation(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_ValidModel_MapsModelToClientApplication(
            [Frozen] CatalogueItemId id,
            AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>();
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication.Object);
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.AdditionalInformation(id, model);

            mockClientApplication.VerifySet(x => x.AdditionalInformation = model.AdditionalInformation);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_ValidModel_CallSaveClientApplicationOnService(
            [Frozen] CatalogueItemId id,
            AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.AdditionalInformation(id, model);

            mockService.Verify(x => x.SaveClientApplication(id, mockClientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_ValidModel_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.AdditionalInformation(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.BrowserBased));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConnectivityAndResolution_ValidId_CallsGetSolutionOnService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ConnectivityAndResolution(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConnectivityAndResolution_NullSolutionFromService_ReturnsBadRequestResponse(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.ConnectivityAndResolution(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConnectivityAndResolution_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new BrowserBasedController(
                mockMapper.Object, mockService.Object);

            await controller.ConnectivityAndResolution(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, ConnectivityAndResolutionModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ConnectivityAndResolution_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockConnectivityAndResolutionModel = new Mock<ConnectivityAndResolutionModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, ConnectivityAndResolutionModel>(mockCatalogueItem))
                .Returns(mockConnectivityAndResolutionModel);
            var controller = new BrowserBasedController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.ConnectivityAndResolution(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockConnectivityAndResolutionModel);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConnectivityAndResolution_InvalidModel_ReturnsViewWithModel([Frozen] CatalogueItemId id)
        {
            var mockConnectivityAndResolutionModel = new Mock<ConnectivityAndResolutionModel>().Object;
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.ConnectivityAndResolution(id, mockConnectivityAndResolutionModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockConnectivityAndResolutionModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConnectivityAndResolution_ValidModel_GetsClientApplicationFromService(
            [Frozen] CatalogueItemId id,
            ConnectivityAndResolutionModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ConnectivityAndResolution(id, model);

            mockService.Verify(x => x.GetClientApplication(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConnectivityAndResolution_NoClientApplicationFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            ConnectivityAndResolutionModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(default(ClientApplication));
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.ConnectivityAndResolution(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConnectivityAndResolution_ValidModel_MapsModelToClientApplication(
            [Frozen] CatalogueItemId id,
            ConnectivityAndResolutionModel model)
        {
            var mockMapper = new Mock<IMapper>();
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(
                mockMapper.Object, mockService.Object);

            await controller.ConnectivityAndResolution(id, model);

            mockMapper.Verify(x => x.Map(model, mockClientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConnectivityAndResolution_ValidModel_CallSaveClientApplicationOnService(
            [Frozen] CatalogueItemId id,
            ConnectivityAndResolutionModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ConnectivityAndResolution(id, model);

            mockService.Verify(x => x.SaveClientApplication(id, mockClientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ConnectivityAndResolution_ValidModel_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            ConnectivityAndResolutionModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.ConnectivityAndResolution(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.BrowserBased));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_ValidId_CallsGetSolutionOnService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.HardwareRequirements(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_NullSolutionFromService_ReturnsBadRequestResponse(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.HardwareRequirements(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new BrowserBasedController(
                mockMapper.Object, mockService.Object);

            await controller.HardwareRequirements(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, HardwareRequirementsModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockHardwareRequirementsModel = new Mock<HardwareRequirementsModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, HardwareRequirementsModel>(mockCatalogueItem))
                .Returns(mockHardwareRequirementsModel);
            var controller = new BrowserBasedController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.HardwareRequirements(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockHardwareRequirementsModel);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_InvalidModel_ReturnsViewWithModel([Frozen] CatalogueItemId id)
        {
            var mockHardwareRequirementsModel = new Mock<HardwareRequirementsModel>().Object;
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.HardwareRequirements(id, mockHardwareRequirementsModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockHardwareRequirementsModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_ValidModel_GetsClientApplicationFromService(
            [Frozen] CatalogueItemId id,
            HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.HardwareRequirements(id, model);

            mockService.Verify(x => x.GetClientApplication(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_NoClientApplicationFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(default(ClientApplication));
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.HardwareRequirements(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_ValidModel_MapsModelToClientApplication(
            [Frozen] CatalogueItemId id,
            HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>();
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication.Object);
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.HardwareRequirements(id, model);

            mockClientApplication.VerifySet(x => x.HardwareRequirements = model.Description);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_ValidModel_CallSaveClientApplicationOnService(
            [Frozen] CatalogueItemId id,
            HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.HardwareRequirements(id, model);

            mockService.Verify(x => x.SaveClientApplication(id, mockClientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_ValidModel_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.HardwareRequirements(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.BrowserBased));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupportedBrowsers_ValidId_CallsGetSolutionOnService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.SupportedBrowsers(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupportedBrowsers_NullSolutionFromService_ReturnsBadRequestResponse(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.SupportedBrowsers(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupportedBrowsers_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new BrowserBasedController(
                mockMapper.Object, mockService.Object);

            await controller.SupportedBrowsers(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, SupportedBrowsersModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupportedBrowsers_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockSupportedBrowsersModel = new Mock<SupportedBrowsersModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, SupportedBrowsersModel>(mockCatalogueItem))
                .Returns(mockSupportedBrowsersModel);
            var controller = new BrowserBasedController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.SupportedBrowsers(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockSupportedBrowsersModel);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_SupportedBrowsers_InvalidModel_ReturnsViewWithModel([Frozen] CatalogueItemId id)
        {
            var mockSupportedBrowsersModel = new Mock<SupportedBrowsersModel>().Object;
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.SupportedBrowsers(id, mockSupportedBrowsersModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockSupportedBrowsersModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SupportedBrowsers_ValidModel_GetsClientApplicationFromService(
            [Frozen] CatalogueItemId id,
            SupportedBrowsersModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.SupportedBrowsers(id, model);

            mockService.Verify(x => x.GetClientApplication(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SupportedBrowsers_NoClientApplicationFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            SupportedBrowsersModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(default(ClientApplication));
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.SupportedBrowsers(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SupportedBrowsers_ValidModel_MapsModelToClientApplication(
            [Frozen] CatalogueItemId id,
            SupportedBrowsersModel model)
        {
            var mockMapper = new Mock<IMapper>();
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(
                mockMapper.Object, mockService.Object);

            await controller.SupportedBrowsers(id, model);

            mockMapper.Verify(x => x.Map(model, mockClientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SupportedBrowsers_ValidModel_CallSaveClientApplicationOnService(
            [Frozen] CatalogueItemId id,
            SupportedBrowsersModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.SupportedBrowsers(id, model);

            mockService.Verify(x => x.SaveClientApplication(id, mockClientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SupportedBrowsers_ValidModel_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            SupportedBrowsersModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.SupportedBrowsers(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.BrowserBased));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_MobileFirstApproach_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.MobileFirstApproach(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_MobileFirstApproach_ServiceReturnsNull_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.MobileFirstApproach(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_MobileFirstApproach_ServiceResponseValid_MapsToModel(
            CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new BrowserBasedController(
                mockMapper.Object, mockService.Object);

            await controller.MobileFirstApproach(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, MobileFirstApproachModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_MobileFirstApproach_ServiceResponseValid_ReturnsExpectedView(
            CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockMobileFirstApproachModel = new Mock<MobileFirstApproachModel>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<CatalogueItem, MobileFirstApproachModel>(mockCatalogueItem))
                .Returns(mockMobileFirstApproachModel);
            var controller = new BrowserBasedController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.MobileFirstApproach(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.Model.Should().Be(mockMobileFirstApproachModel);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_MobileFirstApproach_InvalidModel_ReturnsViewWithModel([Frozen] CatalogueItemId id)
        {
            var mockMobileFirstApproachModel = new Mock<MobileFirstApproachModel>().Object;
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.MobileFirstApproach(id, mockMobileFirstApproachModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockMobileFirstApproachModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MobileFirstApproach_ValidModel_GetsClientApplicationFromService(
            [Frozen] CatalogueItemId id,
            MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.MobileFirstApproach(id, model);

            mockService.Verify(x => x.GetClientApplication(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MobileFirstApproach_NoClientApplicationFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(default(ClientApplication));
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.MobileFirstApproach(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MobileFirstApproach_ValidModel_MapsModelToClientApplication(
            [Frozen] CatalogueItemId id,
            MobileFirstApproachModel model,
            bool? mobileFirstApproach)
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<string, bool?>(model.MobileFirstApproach))
                .Returns(mobileFirstApproach);
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>();
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication.Object);
            var controller = new BrowserBasedController(
                mockMapper.Object, mockService.Object);

            await controller.MobileFirstApproach(id, model);

            mockMapper.Verify(x => x.Map<string, bool?>(model.MobileFirstApproach));
            mockClientApplication.VerifySet(x => x.MobileFirstDesign = mobileFirstApproach);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MobileFirstApproach_ValidModel_CallSaveClientApplicationOnService(
            [Frozen] CatalogueItemId id,
            MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.MobileFirstApproach(id, model);

            mockService.Verify(x => x.SaveClientApplication(id, mockClientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MobileFirstApproach_ValidModel_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.MobileFirstApproach(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.BrowserBased));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_PlugInsOrExtensions_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PlugInsOrExtensions(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PlugInsOrExtensions_ServiceReturnsNull_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PlugInsOrExtensions(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PlugInsOrExtensions_ServiceResponseValid_MapsToModel(
            CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new BrowserBasedController(
                mockMapper.Object, mockService.Object);

            await controller.PlugInsOrExtensions(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, PlugInsOrExtensionsModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PlugInsOrExtensions_ServiceResponseValid_ReturnsExpectedView(
            CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockPlugInsOrExtensionsModel = new Mock<PlugInsOrExtensionsModel>().Object;
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<CatalogueItem, PlugInsOrExtensionsModel>(mockCatalogueItem))
                .Returns(mockPlugInsOrExtensionsModel);
            var controller = new BrowserBasedController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.PlugInsOrExtensions(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.Model.Should().Be(mockPlugInsOrExtensionsModel);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_PlugInsOrExtensions_InvalidModel_ReturnsViewWithModel([Frozen] CatalogueItemId id)
        {
            var mockPlugInsOrExtensionsModel = new Mock<PlugInsOrExtensionsModel>().Object;
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.PlugInsOrExtensions(id, mockPlugInsOrExtensionsModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockPlugInsOrExtensionsModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PlugInsOrExtensions_ValidModel_GetsClientApplicationFromService(
            [Frozen] CatalogueItemId id,
            PlugInsOrExtensionsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PlugInsOrExtensions(id, model);

            mockService.Verify(x => x.GetClientApplication(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PlugInsOrExtensions_NoClientApplicationFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            PlugInsOrExtensionsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(default(ClientApplication));
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PlugInsOrExtensions(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PlugInsOrExtensions_ValidModel_MapsModelToClientApplication(
            [Frozen] CatalogueItemId id,
            PlugInsOrExtensionsModel model)
        {
            var mockMapper = new Mock<IMapper>();
            var mockPlugins = new Mock<Plugins>().Object;
            mockMapper.Setup(x => x.Map<PlugInsOrExtensionsModel, Plugins>(model))
                .Returns(mockPlugins);
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>();
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication.Object);
            var controller = new BrowserBasedController(
                mockMapper.Object, mockService.Object);

            await controller.PlugInsOrExtensions(id, model);

            mockMapper.Verify(x => x.Map<PlugInsOrExtensionsModel, Plugins>(model));
            mockClientApplication.VerifySet(x => x.Plugins = mockPlugins);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PlugInsOrExtensions_ValidModel_CallSaveClientApplicationOnService(
            [Frozen] CatalogueItemId id,
            PlugInsOrExtensionsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PlugInsOrExtensions(id, model);

            mockService.Verify(x => x.SaveClientApplication(id, mockClientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PlugInsOrExtensions_ValidModel_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            PlugInsOrExtensionsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockClientApplication = new Mock<ClientApplication>().Object;
            mockService.Setup(x => x.GetClientApplication(id))
                .ReturnsAsync(mockClientApplication);
            var controller = new BrowserBasedController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PlugInsOrExtensions(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.BrowserBased));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }
    }
}
