using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    public static class AboutSolutionControllerTests
    {
        [Fact]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AboutSolutionController( null,
                    Mock.Of<ISolutionsService>()));
        }

        [Fact]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AboutSolutionController( Mock.Of<IMapper>(),
                    null));
        }

        [Fact]
        public static void Get_Features_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(AboutSolutionController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutSolutionController.Features)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(AboutSolutionController.Features).ToLowerCaseHyphenated());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Features(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_NullSolutionFromService_ReturnsBadRequestResponse(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.Features(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new AboutSolutionController(
                mockMapper.Object, mockService.Object);

            await controller.Features(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, FeaturesModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Features_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockFeaturesModel = new Mock<FeaturesModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, FeaturesModel>(mockCatalogueItem))
                .Returns(mockFeaturesModel);
            var controller = new AboutSolutionController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.Features(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockFeaturesModel);
        }

        [Fact]
        public static void Post_Features_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(AboutSolutionController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutSolutionController.Features)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(AboutSolutionController.Features).ToLowerCaseHyphenated());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Features_InvalidModel_DoesNotCallService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.Features(id, new Mock<FeaturesModel>().Object);

            mockService.Verify(
                s => s.SaveSolutionFeatures(It.IsAny<CatalogueItemId>(), It.IsAny<string[]>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Features_InvalidModel_ReturnsViewWithModel(CatalogueItemId id)
        {
            var mockFeaturesModel = new Mock<FeaturesModel>().Object;
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Features(id, mockFeaturesModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockFeaturesModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Features_ValidModel_MapsModelToArray(CatalogueItemId id)
        {
            var mockFeaturesModel = new Mock<FeaturesModel>().Object;
            var mockMapper = new Mock<IMapper>();
            var controller = new AboutSolutionController(
                mockMapper.Object, Mock.Of<ISolutionsService>());

            await controller.Features(id, mockFeaturesModel);

            mockMapper.Verify(m => m.Map<FeaturesModel, string[]>(mockFeaturesModel));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Features_ValidModel_CallsService(
            [Frozen] CatalogueItemId id,
            FeaturesModel model,
            string[] features)
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<FeaturesModel, string[]>(model))
                .Returns(features);
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                mockMapper.Object, mockService.Object);

            await controller.Features(id, model);

            mockService.Verify(s => s.SaveSolutionFeatures(id, features));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Features_ValidModel_RedirectsToExpectedAction(
            [Frozen] CatalogueItemId id,
            FeaturesModel model,
            string[] features)
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<FeaturesModel, string[]>(model))
                .Returns(features);
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.Features(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be("Solution");
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }

        [Fact]
        public static void Get_ImplementationTimescales_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(AboutSolutionController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutSolutionController.Implementation)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(AboutSolutionController.Implementation).ToLowerCaseHyphenated());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ImplementationTimescales_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Implementation(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ImplementationTimescales_NullSolutionFromService_ReturnsBadRequestResponse(
            CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.Implementation(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ImplementationTimescales_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new AboutSolutionController(
                mockMapper.Object, mockService.Object);

            await controller.Implementation(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, ImplementationTimescalesModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_ImplementationTimescales_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockImplementationTimescalesModel = new Mock<ImplementationTimescalesModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, ImplementationTimescalesModel>(mockCatalogueItem))
                .Returns(mockImplementationTimescalesModel);
            var controller = new AboutSolutionController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.Implementation(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockImplementationTimescalesModel);
        }

        [Fact]
        public static void Post_ImplementationTimescales_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(AboutSolutionController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutSolutionController.Implementation)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(AboutSolutionController.Implementation).ToLowerCaseHyphenated());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ImplementationTimescales_InvalidModel_DoesNotCallService([Frozen] CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.Implementation(id, new Mock<ImplementationTimescalesModel>().Object);

            mockService.Verify(
                s => s.SaveImplementationDetail(It.IsAny<CatalogueItemId>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ImplementationTimescales_InvalidModel_ReturnsViewWithModel([Frozen] CatalogueItemId id)
        {
            var mockImplementationTimescalesModel = new Mock<ImplementationTimescalesModel>().Object;
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Implementation(id, mockImplementationTimescalesModel))
                .As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockImplementationTimescalesModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ImplementationTimescales_ValidModel_CallsService(
            [Frozen] CatalogueItemId id,
            ImplementationTimescalesModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Implementation(id, model);

            mockService.Verify(s => s.SaveImplementationDetail(id, model.Description));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_ImplementationTimescales_ValidModel_RedirectsToExpectedAction(
            [Frozen] CatalogueItemId id,
            ImplementationTimescalesModel model)
        {
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            var actual = (await controller.Implementation(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be("Solution");
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }

        [Fact]
        public static void Get_Integrations_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(AboutSolutionController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutSolutionController.Integrations)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(AboutSolutionController.Integrations).ToLowerCaseHyphenated());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Integrations_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Integrations(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Integrations_NullSolutionFromService_ReturnsBadRequestResponse(
            CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id)).ReturnsAsync(default(CatalogueItem));
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.Integrations(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Integrations_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id)).ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new AboutSolutionController(
                mockMapper.Object, mockService.Object);

            await controller.Integrations(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, IntegrationsModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Integrations_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockIntegrationsModel = new Mock<IntegrationsModel>().Object;
            mockMapper.Setup(m => m.Map<CatalogueItem, IntegrationsModel>(mockCatalogueItem))
                .Returns(mockIntegrationsModel);
            var controller = new AboutSolutionController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.Integrations(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockIntegrationsModel);
        }

        [Fact]
        public static void Post_Integrations_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(AboutSolutionController)
                .GetMethods()
                .First(i => i.Name == nameof(AboutSolutionController.Integrations)
                            && i.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(AboutSolutionController.Integrations).ToLowerCaseHyphenated());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Integrations_InvalidModel_DoesNotCallService([Frozen] CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.Integrations(id, new Mock<IntegrationsModel>().Object);

            mockService.Verify(
                s => s.SaveImplementationDetail(It.IsAny<CatalogueItemId>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Integrations_InvalidModel_ReturnsViewWithModel([Frozen] CatalogueItemId id)
        {
            var mockIntegrationsModel = new Mock<IntegrationsModel>().Object;
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Integrations(id, mockIntegrationsModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockIntegrationsModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Integrations_ValidModel_CallsService(
            [Frozen] CatalogueItemId id,
            IntegrationsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Integrations(id, model);

            mockService.Verify(s => s.SaveIntegrationLink(id, model.Link));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Integrations_ValidModel_RedirectsToExpectedAction(
            [Frozen] CatalogueItemId id,
            IntegrationsModel model)
        {
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            var actual = (await controller.Integrations(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be("Solution");
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Roadmap_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.RoadMap(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Roadmap_NullSolutionFromService_ReturnsBadRequestResponse(
            CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.RoadMap(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Roadmap_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new AboutSolutionController(
                mockMapper.Object, mockService.Object);

            await controller.RoadMap(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, RoadmapModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Roadmap_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockRoadmapModel = new Mock<RoadmapModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, RoadmapModel>(mockCatalogueItem))
                .Returns(mockRoadmapModel);
            var controller = new AboutSolutionController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.RoadMap(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockRoadmapModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Roadmap_InvalidModel_DoesNotCallService([Frozen] CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.RoadMap(id, new Mock<RoadmapModel>().Object);

            mockService.Verify(
                s => s.SaveImplementationDetail(It.IsAny<CatalogueItemId>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Roadmap_InvalidModel_ReturnsViewWithModel([Frozen] CatalogueItemId id)
        {
            var mockRoadmapModel = new Mock<RoadmapModel>().Object;
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.RoadMap(id, mockRoadmapModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockRoadmapModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Roadmap_ValidModel_CallsService([Frozen] CatalogueItemId id, RoadmapModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.RoadMap(id, model);

            mockService.Verify(s => s.SaveRoadMap(id, model.Summary));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_RoadMap_ValidModel_RedirectsToExpectedAction(
            [Frozen] CatalogueItemId id,
            RoadmapModel model)
        {
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            var actual = (await controller.RoadMap(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be("Solution");
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }

        [Fact]
        public static void SolutionDescription_HttpGetAndHttpPostAttribute_ExpectedTemplate()
        {
            typeof(AboutSolutionController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutSolutionController.SolutionDescription)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(AboutSolutionController.SolutionDescription).ToLowerCaseHyphenated());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SolutionDescription_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.SolutionDescription(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SolutionDescription_NullSolutionFromService_ReturnsBadRequestResponse(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.SolutionDescription(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SolutionDescription_ValidId_MapsSolutionToModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new AboutSolutionController(
                mockMapper.Object, mockService.Object);

            await controller.SolutionDescription(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, SolutionDescriptionModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_SolutionDescription_ValidId_ReturnsViewWithModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var catalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(catalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockSolutionDescriptionModel = new Mock<SolutionDescriptionModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, SolutionDescriptionModel>(catalogueItem))
                .Returns(mockSolutionDescriptionModel);
            var controller = new AboutSolutionController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.SolutionDescription(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockSolutionDescriptionModel);
        }

        [Fact]
        public static void Post_SolutionDescription_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(AboutSolutionController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutSolutionController.SolutionDescription)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(AboutSolutionController.SolutionDescription).ToLowerCaseHyphenated());
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SolutionDescription_InvalidModel_DoesNotCallService([Frozen] CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.SolutionDescription(id, new Mock<SolutionDescriptionModel>().Object);

            mockService.Verify(
                s => s.SaveSolutionDescription(It.IsAny<CatalogueItemId>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SolutionDescription_InvalidModel_ReturnsViewWithModel([Frozen] CatalogueItemId id)
        {
            var mockSolutionDescriptionModel = new Mock<SolutionDescriptionModel>().Object;
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.SolutionDescription(id, mockSolutionDescriptionModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockSolutionDescriptionModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SolutionDescription_ValidModel_CallsSaveSolutionDescriptionOnService(
            [Frozen] CatalogueItemId id,
            SolutionDescriptionModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.SolutionDescription(id, model);

            mockService.Verify(s => s.SaveSolutionDescription(id, model.Summary, model.Description, model.Link));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_SolutionDescription_ValidModel_RedirectsToExpectedAction(
            [Frozen] CatalogueItemId id,
            SolutionDescriptionModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.SolutionDescription(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be("Solution");
            actual.RouteValues["solutionId"].Should().Be(model.SolutionId);
        }
    }
}
