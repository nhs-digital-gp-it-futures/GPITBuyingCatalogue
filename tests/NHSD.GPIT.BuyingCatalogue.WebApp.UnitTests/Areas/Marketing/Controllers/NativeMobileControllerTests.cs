﻿using System;
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
using NHSD.GPIT.BuyingCatalogue.Test.Framework;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.NativeMobile;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    public static class NativeMobileControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(NativeMobileController).Should()
                .BeDecoratedWith<AuthorizeAttribute>(p => p.Policy == "AdminOnly");

            typeof(NativeMobileController).Should()
                .BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
        }

        [Fact]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                    _ = new NativeMobileController(
                        null,
                        Mock.Of<ISolutionsService>()))
                .ParamName.Should().Be("mapper");
        }

        [Fact]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new NativeMobileController(
                    Mock.Of<IMapper>(),
                    null))
                .ParamName.Should().Be("solutionsService");
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            await controller.AdditionalInformation(id);

            mockSolutionsService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_NullSolutionFromService_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            var actual = (await controller.AdditionalInformation(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeMobileController(
                mockMapper.Object, mockSolutionsService.Object);

            await controller.AdditionalInformation(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, AdditionalInformationModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalInformation_ValidId_ReturnsExpectedViewWithModel(
            CatalogueItemId id,
            CatalogueItem catalogueItem,
            AdditionalInformationModel additionalInformationModel,
            [Frozen] Mock<IMapper> mockMapper,
            [Frozen] Mock<ISolutionsService> mockSolutionsService,
            NativeMobileController controller)
        {
            mockMapper.Setup(m => m.Map<CatalogueItem, AdditionalInformationModel>(catalogueItem))
                .Returns(additionalInformationModel);
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.AdditionalInformation(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(additionalInformationModel);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_ModelNotValid_DoesNotCallService(
            [Frozen] CatalogueItemId id,
            AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.AdditionalInformation(id, model);

            mockService.Verify(s => s.GetSolution(id), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_ModelNotValid_ReturnsViewWithModel(
            [Frozen] CatalogueItemId id,
            AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.AdditionalInformation(id, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_ModelValid_GetsClientApplicationFromService(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            AdditionalInformationModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            await controller.AdditionalInformation(id, model);

            solutionsServiceMock.Verify(s => s.GetClientApplication(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_NullClientApplicationFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            AdditionalInformationModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(id))
                .ReturnsAsync(default(ClientApplication));
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.AdditionalInformation(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_ModelValid_MapsClientApplication(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            AdditionalInformationModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            await controller.AdditionalInformation(id, model);

            clientApplication.NativeMobileAdditionalInformation.Should().Be(model.AdditionalInformation);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_ModelValid_CallsSaveClientApplication(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            AdditionalInformationModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            await controller.AdditionalInformation(id, model);

            solutionsServiceMock.Verify(s => s.SaveClientApplication(id, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_AdditionalInformation_ModelValid_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            AdditionalInformationModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            var actual = (await controller.AdditionalInformation(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.NativeMobile));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(id);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_Connectivity_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            await controller.Connectivity(id);

            mockSolutionsService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Connectivity_NullSolutionFromService_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            var actual = (await controller.Connectivity(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Connectivity_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeMobileController(
                mockMapper.Object, mockSolutionsService.Object);

            await controller.Connectivity(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, ConnectivityModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Connectivity_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            var mockConnectivityModel = new Mock<ConnectivityModel>().Object;
            mockMapper.Setup(m => m.Map<CatalogueItem, ConnectivityModel>(mockCatalogueItem))
                .Returns(mockConnectivityModel);
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeMobileController(
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.Connectivity(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockConnectivityModel);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_Connectivity_ModelNotValid_DoesNotCallService(
            [Frozen] CatalogueItemId id,
            ConnectivityModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.Connectivity(id, model);

            mockService.Verify(s => s.GetSolution(id), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Connectivity_ModelNotValid_ReturnsViewWithModel(
            [Frozen] CatalogueItemId id,
            ConnectivityModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Connectivity(id, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Connectivity_ModelValid_GetsClientApplicationFromService(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            ConnectivityModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            await controller.Connectivity(id, model);

            solutionsServiceMock.Verify(s => s.GetClientApplication(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Connectivity_NullClientApplicationFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            ConnectivityModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(id))
                .ReturnsAsync(default(ClientApplication));
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.Connectivity(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Connectivity_ModelValid_MapsClientApplication(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            MobileConnectionDetails details,
            NativeMobileController controller,
            ConnectivityModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);
            mapperMock.Setup(m => m.Map<ConnectivityModel, MobileConnectionDetails>(model)).Returns(details);

            await controller.Connectivity(id, model);

            mapperMock.Verify(m => m.Map<ConnectivityModel, MobileConnectionDetails>(model));
            clientApplication.MobileConnectionDetails.Should().Be(details);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Connectivity_ModelValid_CallsSaveClientApplication(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            ConnectivityModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            await controller.Connectivity(id, model);

            solutionsServiceMock.Verify(s => s.SaveClientApplication(id, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Connectivity_ModelValid_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            ConnectivityModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            var actual = (await controller.Connectivity(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.NativeMobile));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.NativeMobile));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(id);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            await controller.HardwareRequirements(id);

            mockSolutionsService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_NullSolutionFromService_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            var actual = (await controller.HardwareRequirements(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeMobileController(
                mockMapper.Object, mockSolutionsService.Object);

            await controller.HardwareRequirements(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, HardwareRequirementsModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HardwareRequirements_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            var mockHardwareRequirementsModel = new Mock<HardwareRequirementsModel>().Object;
            mockMapper.Setup(m => m.Map<CatalogueItem, HardwareRequirementsModel>(mockCatalogueItem))
                .Returns(mockHardwareRequirementsModel);
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeMobileController(
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.HardwareRequirements(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockHardwareRequirementsModel);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_ModelNotValid_DoesNotCallService(
            [Frozen] CatalogueItemId id,
            HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.HardwareRequirements(id, model);

            mockService.Verify(s => s.GetSolution(id), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_ModelNotValid_ReturnsViewWithModel(
            [Frozen] CatalogueItemId id,
            HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.HardwareRequirements(id, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_ModelValid_GetsClientApplicationFromService(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            HardwareRequirementsModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            await controller.HardwareRequirements(id, model);

            solutionsServiceMock.Verify(s => s.GetClientApplication(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_NullClientApplicationFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            HardwareRequirementsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(id))
                .ReturnsAsync(default(ClientApplication));
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.HardwareRequirements(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_ModelValid_MapsClientApplication(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            HardwareRequirementsModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            await controller.HardwareRequirements(id, model);

            clientApplication.NativeMobileHardwareRequirements.Should().Be(model.Description);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_ModelValid_CallsSaveClientApplication(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            HardwareRequirementsModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            await controller.HardwareRequirements(id, model);

            solutionsServiceMock.Verify(s => s.SaveClientApplication(id, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_HardwareRequirements_ModelValid_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            HardwareRequirementsModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            var actual = (await controller.HardwareRequirements(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.NativeMobile));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(id);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_MemoryAndStorage_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            await controller.MemoryAndStorage(id);

            mockSolutionsService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_MemoryAndStorage_NullSolutionFromService_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            var actual = (await controller.MemoryAndStorage(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_MemoryAndStorage_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeMobileController(
                mockMapper.Object, mockSolutionsService.Object);

            await controller.MemoryAndStorage(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, MemoryAndStorageModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_MemoryAndStorage_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            var mockMemoryAndStorageModel = new Mock<MemoryAndStorageModel>().Object;
            mockMapper.Setup(m => m.Map<CatalogueItem, MemoryAndStorageModel>(mockCatalogueItem))
                .Returns(mockMemoryAndStorageModel);
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeMobileController(
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.MemoryAndStorage(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockMemoryAndStorageModel);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_MemoryAndStorage_ModelNotValid_DoesNotCallService(
            [Frozen] CatalogueItemId id,
            MemoryAndStorageModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.MemoryAndStorage(id, model);

            mockService.Verify(s => s.GetSolution(id), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MemoryAndStorage_ModelNotValid_ReturnsViewWithModel(
            [Frozen] CatalogueItemId id,
            MemoryAndStorageModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.MemoryAndStorage(id, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MemoryAndStorage_ModelValid_GetsClientApplicationFromService(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            MemoryAndStorageModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            await controller.MemoryAndStorage(id, model);

            solutionsServiceMock.Verify(s => s.GetClientApplication(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MemoryAndStorage_NullClientApplicationFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            MemoryAndStorageModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(id))
                .ReturnsAsync(default(ClientApplication));
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.MemoryAndStorage(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MemoryAndStorage_ModelValid_MapsClientApplication(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            MobileMemoryAndStorage storage,
            NativeMobileController controller,
            MemoryAndStorageModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);
            mapperMock.Setup(m => m.Map<MemoryAndStorageModel, MobileMemoryAndStorage>(model)).Returns(storage);

            await controller.MemoryAndStorage(id, model);

            mapperMock.Verify(m => m.Map<MemoryAndStorageModel, MobileMemoryAndStorage>(model));
            clientApplication.MobileMemoryAndStorage.Should().Be(storage);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MemoryAndStorage_ModelValid_CallsSaveClientApplication(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            MemoryAndStorageModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            await controller.MemoryAndStorage(id, model);

            solutionsServiceMock.Verify(s => s.SaveClientApplication(id, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MemoryAndStorage_ModelValid_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            MemoryAndStorageModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            var actual = (await controller.MemoryAndStorage(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.NativeMobile));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.NativeMobile));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(id);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_MobileFirstApproach_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            await controller.MobileFirstApproach(id);

            mockSolutionsService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_MobileFirstApproach_NullSolutionFromService_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            var actual = (await controller.MobileFirstApproach(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_MobileFirstApproach_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeMobileController(
                mockMapper.Object, mockSolutionsService.Object);

            await controller.MobileFirstApproach(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, MobileFirstApproachModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_MobileFirstApproach_ValidId_ReturnsExpectedViewWithModel(
            [Frozen] CatalogueItemId id,
            CatalogueItem catalogueItem,
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            MobileFirstApproachModel model,
            NativeMobileController controller)
        {
            mapperMock.Setup(m => m.Map<CatalogueItem, MobileFirstApproachModel>(catalogueItem)).Returns(model);
            solutionsServiceMock.Setup(s => s.GetSolution(id)).ReturnsAsync(catalogueItem);

            var actual = (await controller.MobileFirstApproach(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(model);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_MobileFirstApproach_ModelNotValid_DoesNotCallService(
            [Frozen] CatalogueItemId id,
            MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.MobileFirstApproach(id, model);

            mockService.Verify(s => s.GetSolution(id), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MobileFirstApproach_ModelNotValid_ReturnsViewWithModel(
            [Frozen] CatalogueItemId id,
            MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.MobileFirstApproach(id, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MobileFirstApproach_ModelValid_GetsClientApplicationFromService(
            [Frozen] CatalogueItemId id,
            MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.MobileFirstApproach(id, model);

            mockService.Verify(s => s.GetClientApplication(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MobileFirstApproach_NullClientApplicationFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            MobileFirstApproachModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(id))
                .ReturnsAsync(default(ClientApplication));
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.MobileFirstApproach(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonInlineAutoData(null, null)]
        [CommonInlineAutoData("", null)]
        [CommonInlineAutoData("\t", null)]
        [CommonInlineAutoData("No", false)]
        [CommonInlineAutoData("Yes", true)]
        public static async Task Post_MobileFirstApproach_ModelValid_MapsClientApplication(
            string mobileFirstApproach,
            bool? expectedMobileFirstDesign,
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            MobileFirstApproachModel model)
        {
            model.MobileFirstApproach = mobileFirstApproach;
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            await controller.MobileFirstApproach(id, model);

            clientApplication.NativeMobileFirstDesign.Should().Be(expectedMobileFirstDesign);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MobileFirstApproach_ModelValid_CallsSaveClientApplication(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            MobileFirstApproachModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            await controller.MobileFirstApproach(id, model);

            solutionsServiceMock.Verify(s => s.SaveClientApplication(id, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_MobileFirstApproach_ModelValid_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            MobileFirstApproachModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            var actual = (await controller.MobileFirstApproach(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.NativeMobile));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(id);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_OperatingSystems_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            await controller.OperatingSystems(id);

            mockSolutionsService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OperatingSystems_NullSolutionFromService_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            var actual = (await controller.OperatingSystems(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OperatingSystems_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeMobileController(
                mockMapper.Object, mockSolutionsService.Object);

            await controller.OperatingSystems(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, OperatingSystemsModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OperatingSystems_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            var mockOperatingSystemsModel = new Mock<OperatingSystemsModel>().Object;
            mockMapper.Setup(m => m.Map<CatalogueItem, OperatingSystemsModel>(mockCatalogueItem))
                .Returns(mockOperatingSystemsModel);
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeMobileController(
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.OperatingSystems(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockOperatingSystemsModel);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_OperatingSystems_ModelNotValid_DoesNotCallService(
            [Frozen] CatalogueItemId id,
            OperatingSystemsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.OperatingSystems(id, model);

            mockService.Verify(s => s.GetSolution(id), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OperatingSystems_ModelNotValid_ReturnsViewWithModel(
            [Frozen] CatalogueItemId id,
            OperatingSystemsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.OperatingSystems(id, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OperatingSystems_ModelValid_GetsClientApplicationFromService(
            [Frozen] CatalogueItemId id,
            OperatingSystemsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.OperatingSystems(id, model);

            mockService.Verify(s => s.GetClientApplication(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OperatingSystems_NullClientApplicationFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            OperatingSystemsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(id))
                .ReturnsAsync(default(ClientApplication));
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.OperatingSystems(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OperatingSystems_ModelValid_MapsClientApplication(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            MobileOperatingSystems mobileOperatingSystems,
            NativeMobileController controller,
            OperatingSystemsModel model)
        {
            mapperMock.Setup(m => m.Map<OperatingSystemsModel, MobileOperatingSystems>(model)).Returns(mobileOperatingSystems);
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            await controller.OperatingSystems(id, model);

            clientApplication.MobileOperatingSystems.Should().BeEquivalentTo(mobileOperatingSystems);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OperatingSystems_ModelValid_CallsSaveClientApplication(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            OperatingSystemsModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            await controller.OperatingSystems(id, model);

            solutionsServiceMock.Verify(s => s.SaveClientApplication(id, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OperatingSystems_ModelValid_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            OperatingSystemsModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            var actual = (await controller.OperatingSystems(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.NativeMobile));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(id);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_ThirdParty_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            await controller.ThirdParty(id);

            mockSolutionsService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ThirdParty_NullSolutionFromService_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockSolutionsService = new Mock<ISolutionsService>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockSolutionsService.Object);

            var actual = (await controller.ThirdParty(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ThirdParty_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeMobileController(
                mockMapper.Object, mockSolutionsService.Object);

            await controller.ThirdParty(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, ThirdPartyModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ThirdParty_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionsService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            var mockThirdPartyModel = new Mock<ThirdPartyModel>().Object;
            mockMapper.Setup(m => m.Map<CatalogueItem, ThirdPartyModel>(mockCatalogueItem))
                .Returns(mockThirdPartyModel);
            mockSolutionsService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new NativeMobileController(
                mockMapper.Object, mockSolutionsService.Object);

            var actual = (await controller.ThirdParty(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockThirdPartyModel);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_ThirdParty_ModelNotValid_DoesNotCallService(
            [Frozen] CatalogueItemId id,
            ThirdPartyModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.ThirdParty(id, model);

            mockService.Verify(s => s.GetSolution(id), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ThirdParty_ModelNotValid_ReturnsViewWithModel(
            [Frozen] CatalogueItemId id,
            ThirdPartyModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.ThirdParty(id, model)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(model);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ThirdParty_ModelValid_GetsClientApplicationFromService(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            ThirdPartyModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            await controller.ThirdParty(id, model);

            solutionsServiceMock.Verify(s => s.GetClientApplication(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ThirdParty_NullClientApplicationFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            ThirdPartyModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetClientApplication(id))
                .ReturnsAsync(default(ClientApplication));
            var controller = new NativeMobileController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.ThirdParty(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Client Application found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ThirdParty_ModelValid_MapsClientApplication(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<IMapper> mapperMock,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            MobileThirdParty mobileThirdParty,
            NativeMobileController controller,
            ThirdPartyModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);
            mapperMock.Setup(m => m.Map<ThirdPartyModel, MobileThirdParty>(model)).Returns(mobileThirdParty);

            await controller.ThirdParty(id, model);

            mapperMock.Verify(m => m.Map<ThirdPartyModel, MobileThirdParty>(model));
            clientApplication.MobileThirdParty.Should().BeEquivalentTo(mobileThirdParty);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ThirdParty_ModelValid_CallsSaveClientApplication(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            ThirdPartyModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            await controller.ThirdParty(id, model);

            solutionsServiceMock.Verify(s => s.SaveClientApplication(id, clientApplication));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ThirdParty_ModelValid_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            ClientApplication clientApplication,
            [Frozen] Mock<ISolutionsService> solutionsServiceMock,
            NativeMobileController controller,
            ThirdPartyModel model)
        {
            solutionsServiceMock.Setup(s => s.GetClientApplication(id)).ReturnsAsync(clientApplication);

            var actual = (await controller.ThirdParty(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(ClientApplicationTypeController.NativeMobile));
            actual.ControllerName.Should().Be(typeof(ClientApplicationTypeController).ControllerName());
            actual.RouteValues["solutionId"].Should().Be(id);
        }
    }
}
