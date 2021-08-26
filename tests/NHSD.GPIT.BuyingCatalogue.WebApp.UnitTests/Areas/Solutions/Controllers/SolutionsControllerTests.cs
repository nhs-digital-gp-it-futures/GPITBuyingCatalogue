using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    public static class SolutionsControllerTests
    {
        [Fact]
        public static void Class_AreaAttribute_ExpectedAreaName()
        {
            typeof(SolutionsController)
                .GetCustomAttribute<AreaAttribute>()
                .RouteValue.Should()
                .Be("Solutions");
        }

        [Fact]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                    () => _ = new SolutionsController(null, Mock.Of<ISolutionsService>()))
                .ParamName.Should()
                .Be("mapper");
        }

        [Fact]
        public static void Constructor_NullService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new SolutionsController(Mock.Of<IMapper>(), null))
                .ParamName.Should()
                .Be("solutionsService");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AssociatedServices_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            await controller.AssociatedServices(id);

            mockService.Verify(s => s.GetSolutionWithAllAssociatedServices(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AssociatedServices_NullSolutionForId_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolutionWithAllAssociatedServices(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.AssociatedServices(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AssociatedServices_ValidSolutionForId_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionWithAllAssociatedServices(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            await controller.AssociatedServices(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, AssociatedServicesModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AssociatedServices_ValidSolutionForId_ReturnsExpectedViewResult(CatalogueItemId id)
        {
            var mockAssociatedServicesModel = new Mock<AssociatedServicesModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionWithAllAssociatedServices(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, AssociatedServicesModel>(mockCatalogueItem))
                .Returns(mockAssociatedServicesModel);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            var actual = (await controller.AssociatedServices(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockAssociatedServicesModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Capabilities_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            await controller.Capabilities(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Capabilities_NullSolutionForId_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.Capabilities(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Capabilities_ValidSolutionForId_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            await controller.Capabilities(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, CapabilitiesViewModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Capabilities_ValidSolutionForId_ReturnsExpectedViewResult(CatalogueItemId id)
        {
            var mockCapabilitiesViewModel = new Mock<CapabilitiesViewModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, CapabilitiesViewModel>(mockCatalogueItem))
                .Returns(mockCapabilitiesViewModel);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            var actual = (await controller.Capabilities(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockCapabilitiesViewModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ClientApplicationTypes_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            await controller.ClientApplicationTypes(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ClientApplicationTypes_NullSolutionForId_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.ClientApplicationTypes(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ClientApplicationTypes_ValidSolutionForId_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            await controller.ClientApplicationTypes(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, ClientApplicationTypesModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ClientApplicationTypes_ValidSolutionForId_ReturnsExpectedViewResult(CatalogueItemId id)
        {
            var mockSolutionClientApplicationTypesModel = new Mock<ClientApplicationTypesModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, ClientApplicationTypesModel>(mockCatalogueItem))
                .Returns(mockSolutionClientApplicationTypesModel);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            var actual = (await controller.ClientApplicationTypes(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionClientApplicationTypesModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CheckEpics_ValidId_ResultAsExpected(
            CatalogueItemId catalogueItemId,
            int capabilityId,
            CatalogueItemCapability itemCapability,
            string solutionName)
        {
            itemCapability.CapabilityId = capabilityId;

            var mockCatalogueItem = new Mock<CatalogueItem>();
            mockCatalogueItem.SetupGet(c => c.Name)
                .Returns(solutionName);
            mockCatalogueItem.Setup(c => c.CatalogueItemCapability(capabilityId))
                .Returns(itemCapability);

            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolutionCapability(catalogueItemId, capabilityId))
                .ReturnsAsync(mockCatalogueItem.Object);

            var mockSolutionCheckEpicsModel = new Mock<SolutionCheckEpicsModel>();
            mockSolutionCheckEpicsModel.Setup(s => s.WithSolutionName(solutionName))
                .Returns(mockSolutionCheckEpicsModel.Object);

            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<CatalogueItemCapability, SolutionCheckEpicsModel>(itemCapability))
                .Returns(mockSolutionCheckEpicsModel.Object);

            var actual = (await new SolutionsController(
                mockMapper.Object,
                mockService.Object).CheckEpics(catalogueItemId, capabilityId)).As<ViewResult>();

            mockService.Verify(s => s.GetSolutionCapability(catalogueItemId, capabilityId));
            mockCatalogueItem.Verify(c => c.CatalogueItemCapability(capabilityId));
            mockMapper.Verify(
                m => m.Map<CatalogueItemCapability, SolutionCheckEpicsModel>(itemCapability));
            mockCatalogueItem.VerifyGet(c => c.Name);
            mockSolutionCheckEpicsModel.Verify(m => m.WithSolutionName(solutionName));

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionCheckEpicsModel.Object);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CheckEpics_NullSolutionForCapabilityId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            int capabilityId)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolutionCapability(catalogueItemId, capabilityId))
                .ReturnsAsync(default(CatalogueItem));

            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.CheckEpics(catalogueItemId, capabilityId)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should()
                .Be($"No Catalogue Item found for Id: {catalogueItemId} with Capability Id: {capabilityId}");
        }

        [Fact]
        public static void Get_CheckEpics_RouteAttribute_ExpectedTemplate()
        {
            typeof(SolutionsController)
                .GetMethod(nameof(SolutionsController.CheckEpics))
                .GetCustomAttribute<HttpGetAttribute>()
                .Template.Should()
                .Be("{solutionId}/capability/{capabilityId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CheckEpicsAdditionalServices_ValidIds_ResultAsExpected(
            CatalogueItemId catalogueItemId,
            CatalogueItemId catalogueItemIdAdditional,
            int capabilityId,
            CatalogueItemCapability itemCapability,
            string solutionName)
        {
            itemCapability.CapabilityId = capabilityId;

            var mockCatalogueItem = new Mock<CatalogueItem>();
            mockCatalogueItem.SetupGet(c => c.Name)
                .Returns(solutionName);
            mockCatalogueItem.Setup(c => c.CatalogueItemCapability(capabilityId))
                .Returns(itemCapability);
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(
                    s => s.GetAdditionalServiceCapability(catalogueItemIdAdditional, capabilityId))
                .ReturnsAsync(mockCatalogueItem.Object);

            var mockSolutionCheckEpicsModel = new Mock<SolutionCheckEpicsModel>();
            mockSolutionCheckEpicsModel.Setup(
                    s => s.WithItems(catalogueItemId, catalogueItemIdAdditional, solutionName))
                .Returns(mockSolutionCheckEpicsModel.Object);
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(m => m.Map<CatalogueItemCapability, SolutionCheckEpicsModel>(itemCapability))
                .Returns(mockSolutionCheckEpicsModel.Object);

            var solutionDetailsController = new SolutionsController(mockMapper.Object, mockService.Object);

            await solutionDetailsController
                .CheckEpicsAdditionalServices(catalogueItemId, catalogueItemIdAdditional, capabilityId);

            mockService.Verify(
                s => s.GetAdditionalServiceCapability(catalogueItemIdAdditional, capabilityId));
            mockCatalogueItem.Verify(c => c.CatalogueItemCapability(capabilityId));
            mockMapper.Verify(
                m => m.Map<CatalogueItemCapability, SolutionCheckEpicsModel>(itemCapability));
            mockCatalogueItem.VerifyGet(c => c.Name);
            mockSolutionCheckEpicsModel.Verify(
                s => s.WithItems(catalogueItemId, catalogueItemIdAdditional, solutionName));
            mockSolutionCheckEpicsModel.Verify(
                m => m.WithItems(catalogueItemId, catalogueItemIdAdditional, solutionName));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CheckEpicsAdditionalServices_NullSolutionForCapabilityId_ReturnsBadRequestResult(
            CatalogueItemId catalogueItemId,
            CatalogueItemId catalogueItemIdAdditional,
            int capabilityId)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolutionCapability(catalogueItemId, capabilityId))
                .ReturnsAsync(default(CatalogueItem));

            var solutionDetailsController = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await solutionDetailsController
                    .CheckEpicsAdditionalServices(catalogueItemId, catalogueItemIdAdditional, capabilityId))
                .As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {catalogueItemId} with Capability Id: {capabilityId}");
        }

        [Fact]
        public static void Get_CheckEpicsAdditionalServices_RouteAttribute_ExpectedTemplate()
        {
            typeof(SolutionsController)
                .GetMethod(nameof(SolutionsController.CheckEpicsAdditionalServices))
                .GetCustomAttribute<HttpGetAttribute>()
                .Template.Should()
                .Be(
                    "{solutionId}/additional-services/{additionalServiceId}/capability/{capabilityId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Description_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            await controller.Description(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Description_NullSolutionForId_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.Description(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Description_ValidSolutionForId_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            await controller.Description(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, SolutionDescriptionModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Description_ValidSolutionForId_ReturnsExpectedViewResult(CatalogueItemId id)
        {
            var mockSolutionDescriptionModel = new Mock<SolutionDescriptionModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, SolutionDescriptionModel>(mockCatalogueItem))
                .Returns(mockSolutionDescriptionModel);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            var actual = (await controller.Description(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionDescriptionModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_ValidId_InvokesGetSolutionOverview(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            await controller.Features(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_NullSolutionForId_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.Features(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_ValidSolutionForId_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            await controller.Features(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, SolutionFeaturesModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_ValidSolutionForId_ReturnsExpectedViewResult(CatalogueItemId id)
        {
            var mockSolutionFeaturesModel = new Mock<SolutionFeaturesModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, SolutionFeaturesModel>(mockCatalogueItem))
                .Returns(mockSolutionFeaturesModel);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            var actual = (await controller.Features(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionFeaturesModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HostingType_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            await controller.HostingType(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HostingType_NullSolutionForId_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.HostingType(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HostingType_ValidSolutionForId_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            await controller.HostingType(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, HostingTypesModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_HostingType_ValidSolutionForId_ReturnsExpectedViewResult(CatalogueItemId id)
        {
            var mockSolutionHostingTypeModel = new Mock<HostingTypesModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, HostingTypesModel>(mockCatalogueItem))
                .Returns(mockSolutionHostingTypeModel);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            var actual = (await controller.HostingType(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionHostingTypeModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ImplementationTimescales_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            await controller.Implementation(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ImplementationTimescales_NullSolutionForId_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.Implementation(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ImplementationTimescales_ValidSolutionForId_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            await controller.Implementation(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, ImplementationTimescalesModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ImplementationTimescales_ValidSolutionForId_ReturnsExpectedViewResult(CatalogueItemId id)
        {
            var mockSolutionImplementationTimescalesModel = new Mock<ImplementationTimescalesModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, ImplementationTimescalesModel>(mockCatalogueItem))
                .Returns(mockSolutionImplementationTimescalesModel);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            var actual = (await controller.Implementation(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionImplementationTimescalesModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ListPrice_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            await controller.ListPrice(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ListPrice_NullSolutionForId_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.ListPrice(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ListPrice_ValidSolutionForId_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            await controller.ListPrice(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, ListPriceModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ListPrice_ValidSolutionForId_ReturnsExpectedViewResult(CatalogueItemId id)
        {
            var mockSolutionListPriceModel = new Mock<ListPriceModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, ListPriceModel>(mockCatalogueItem))
                .Returns(mockSolutionListPriceModel);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            var actual = (await controller.ListPrice(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionListPriceModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Interoperability_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            await controller.Interoperability(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Interoperability_NullSolutionForId_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.Interoperability(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Interoperability_ValidSolutionForId_ReturnsExpectedViewResult(
            CatalogueItemId id,
            CatalogueItem catalogueItem,
            [Frozen] Mock<ISolutionsService> mockSolutionService,
            SolutionsController controller)
        {
            catalogueItem.Solution.Integrations = GetIntegrationsJson();

            var expectedViewData = new InteroperabilityModel(catalogueItem);

            mockSolutionService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(catalogueItem);

            var actual = (await controller.Interoperability(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().BeEquivalentTo(expectedViewData);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierDetails_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            await controller.SupplierDetails(id);

            mockService.Verify(s => s.GetSolutionOverview(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierDetails_NullSolutionForId_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.SupplierDetails(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierDetails_ValidSolutionForId_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            await controller.SupplierDetails(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, SolutionSupplierDetailsModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierDetails_ValidSolutionForId_ReturnsExpectedViewResult(CatalogueItemId id)
        {
            var mockSolutionDescriptionModel = new Mock<SolutionSupplierDetailsModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, SolutionSupplierDetailsModel>(mockCatalogueItem))
                .Returns(mockSolutionDescriptionModel);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            var actual = (await controller.SupplierDetails(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionDescriptionModel);
        }

        [Fact]
        public static void Get_AdditionalServices_RouteAttribute_ExpectedTemplate()
        {
            typeof(SolutionsController)
                .GetMethod(nameof(SolutionsController.AdditionalServices))
                .GetCustomAttribute<HttpGetAttribute>()
                .Template.Should()
                .Be("{solutionId}/additional-services");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalServices_ValidId_InvokesGetSolution(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            await controller.AdditionalServices(id);

            mockService.Verify(s => s.GetSolutionWithAllAdditionalServices(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalServices_NullSolutionForId_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolutionWithAllAdditionalServices(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionsController(
                Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.AdditionalServices(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalServices_ValidSolutionForId_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionWithAllAdditionalServices(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            await controller.AdditionalServices(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, AdditionalServicesModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalServices_ValidSolutionForId_ReturnsExpectedViewResult(CatalogueItemId id)
        {
            var mockAdditionalServicesModel = new AdditionalServicesModel();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionWithAllAdditionalServices(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, AdditionalServicesModel>(mockCatalogueItem))
                .Returns(mockAdditionalServicesModel);
            var controller = new SolutionsController(
                mockMapper.Object,
                mockService.Object);

            var actual = (await controller.AdditionalServices(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockAdditionalServicesModel);
        }

        private static string GetIntegrationsJson()
        {
            var integrations = new List<Integration>
            {
                new Integration
                {
                    Id = Guid.NewGuid(),
                    IntegrationType = "IM1",
                    Qualifier = "Bulk",
                    IntegratesWith = "Audit+",
                    Description = "Audit+ utilises a bulk extraction of full clinical records (including confidential and deceased patients) from EMIS Web to provide General Practices with a crossplatform clinical decision support and management tool; supporting QOF performance management, improvement and NHS Health Checks.",
                },
                new Integration
                {
                    Id = Guid.NewGuid(),
                    IntegrationType = "GP Connect",
                    Qualifier = "Access Record HTML",
                    AdditionalInformation = "EMIS Web received Full Roll Out Approval from NHS Digital for GP Connect HTML View Provision on 20/06/19",
                },
            };

            return JsonConvert.SerializeObject(integrations);
        }
    }
}
