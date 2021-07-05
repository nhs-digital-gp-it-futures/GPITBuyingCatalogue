using System;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    public static class SolutionDetailsControllerTests
    {
        [Fact]
        public static void Class_AreaAttribute_ExpectedAreaName()
        {
            typeof(SolutionDetailsController)
                .GetCustomAttribute<AreaAttribute>()
                .RouteValue.Should()
                .Be("Solutions");
        }

        [Fact]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(
                    () => _ = new SolutionDetailsController(null, Mock.Of<ISolutionsService>()))
                .ParamName.Should()
                .Be("mapper");
        }

        [Fact]
        public static void Constructor_NullService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new SolutionDetailsController(Mock.Of<IMapper>(), null))
                .ParamName.Should()
                .Be("solutionsService");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AssociatedServices_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(mockMapper.Object,
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
            var controller = new SolutionDetailsController(mockMapper.Object,
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(mockMapper.Object,
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
            var controller = new SolutionDetailsController(mockMapper.Object,
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(mockMapper.Object,
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
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            var actual = (await controller.ClientApplicationTypes(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionClientApplicationTypesModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CheckEpics_ValidIds_GetsSolutionAndCapabilityFromService(CatalogueItemId catalogueItemId, Guid capabilityId)
        {
            var mockSolutionCheckEpicsModel = new Mock<SolutionCheckEpicsModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionCapability = new Mock<SolutionCapability>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionCapability(catalogueItemId, capabilityId))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<SolutionCapability, SolutionCheckEpicsModel>(mockSolutionCapability))
                .Returns(mockSolutionCheckEpicsModel);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            await controller.CheckEpics(catalogueItemId, capabilityId);

            mockService.Verify(s => s.GetSolutionCapability(catalogueItemId, capabilityId));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CheckEpics_NullSolutionForCapabilityId_ReturnsBadRequestResult(CatalogueItemId catalogueItemId, Guid capabilityId)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolutionCapability(catalogueItemId, capabilityId))
                .ReturnsAsync(default(CatalogueItem));

            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.CheckEpics(catalogueItemId, capabilityId)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {catalogueItemId} with Capability Id: {capabilityId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CheckEpics_ValidIds_MapsToModel(CatalogueItemId catalogueItemId, Guid capabilityId)
        {
            var mockSolutionCheckEpicsModel = new Mock<SolutionCheckEpicsModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionCapability = new Mock<SolutionCapability>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionCapability(catalogueItemId, capabilityId))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<SolutionCapability, SolutionCheckEpicsModel>(mockSolutionCapability))
                .Returns(mockSolutionCheckEpicsModel);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            await controller.CheckEpics(catalogueItemId, capabilityId);

            mockMapper.Verify(m => m.Map<SolutionCapability, SolutionCheckEpicsModel>(It.IsAny<SolutionCapability>()));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_CheckEpics_ValidIds_ReturnsExpectedViewResult(CatalogueItemId catalogueItemId, Guid capabilityId)
        {
            var mockSolutionCheckEpicsModel = new Mock<SolutionCheckEpicsModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockSolutionCapability = new Mock<SolutionCapability>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionCapability(catalogueItemId, capabilityId))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<SolutionCapability, SolutionCheckEpicsModel>(It.IsAny<SolutionCapability>()))
                .Returns(mockSolutionCheckEpicsModel);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            var actual = (await controller.CheckEpics(catalogueItemId, capabilityId)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionCheckEpicsModel);
        }

        [Fact]
        public static void Get_CheckEpics_RouteAttribute_ExpectedTemplate()
        {
            typeof(SolutionDetailsController)
                .GetMethod(nameof(SolutionDetailsController.CheckEpics))
                .GetCustomAttribute<RouteAttribute>()
                .Template.Should()
                .Be("futures/{catalogueItemId}/capability/{capabilityId:guid}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Description_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(mockMapper.Object,
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
            var controller = new SolutionDetailsController(mockMapper.Object,
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(mockMapper.Object,
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
            var controller = new SolutionDetailsController(mockMapper.Object,
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(mockMapper.Object,
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
            var controller = new SolutionDetailsController(mockMapper.Object,
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(mockMapper.Object,
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
            var controller = new SolutionDetailsController(mockMapper.Object,
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(mockMapper.Object,
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
            var controller = new SolutionDetailsController(mockMapper.Object,
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.Interoperability(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Interoperability_ValidSolutionForId_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            await controller.Interoperability(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, InteroperabilityModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Interoperability_ValidSolutionForId_ReturnsExpectedViewResult(CatalogueItemId id)
        {
            var mockSolutionInteroperabilityModel = new Mock<InteroperabilityModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionOverview(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, InteroperabilityModel>(mockCatalogueItem))
                .Returns(mockSolutionInteroperabilityModel);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            var actual = (await controller.Interoperability(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionInteroperabilityModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SupplierDetails_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(mockMapper.Object,
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
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            var actual = (await controller.SupplierDetails(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionDescriptionModel);
        }

        [Fact]
        public static void Get_SupplierDetails_RouteAttribute_ExpectedTemplate()
        {
            typeof(SolutionDetailsController)
                .GetMethod(nameof(SolutionDetailsController.SupplierDetails))
                .GetCustomAttribute<RouteAttribute>()
                .Template.Should()
                .Be("futures/{id}/supplier-details");
        }

        [Fact]
        public static void Get_AdditionalServices_RouteAttribute_ExpectedTemplate()
        {
            typeof(SolutionDetailsController)
                .GetMethod(nameof(SolutionDetailsController.AdditionalServices))
                .GetCustomAttribute<RouteAttribute>()
                .Template.Should()
                .Be("futures/{id}/additional-services");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalServices_ValidId_InvokesGetSolution(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(Mock.Of<IMapper>(),
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
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            await controller.AdditionalServices(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, AdditionalServicesModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_AdditionalServices_ValidSolutionForId_ReturnsExpectedViewResult(CatalogueItemId id)
        {
            var mockAdditionalServicesModel = new Mock<AdditionalServicesModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolutionWithAllAdditionalServices(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, AdditionalServicesModel>(mockCatalogueItem))
                .Returns(mockAdditionalServicesModel);
            var controller = new SolutionDetailsController(mockMapper.Object,
                mockService.Object);

            var actual = (await controller.AdditionalServices(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockAdditionalServicesModel);
        }
    }
}
