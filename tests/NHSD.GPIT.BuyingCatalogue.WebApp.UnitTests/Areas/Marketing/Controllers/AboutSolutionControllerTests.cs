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
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class AboutSolutionControllerTests
    {
        private static string[] InvalidStrings = {null, string.Empty, "    "};

        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AboutSolutionController).Should()
                .BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
            typeof(AboutSolutionController).Should()
                .BeDecoratedWith<RouteAttribute>(x => x.Template == "marketing/supplier/solution/{id}/section");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AboutSolutionController(null, Mock.Of<IMapper>(), Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(), null,
                    Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(), Mock.Of<IMapper>(),
                    null));
        }

        [Test]
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

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_Features_InvalidId_ThrowsException(string id)
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.Features(id));
        }

        [Test, AutoData]
        public static async Task Get_Features_ValidId_GetsSolutionFromService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Features(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_Features_NullSolutionFromService_ReturnsBadRequestResponse(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.Features(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_Features_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                mockMapper.Object, mockService.Object);

            await controller.Features(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, FeaturesModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_Features_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockFeaturesModel = new Mock<FeaturesModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, FeaturesModel>(mockCatalogueItem))
                .Returns(mockFeaturesModel);
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.Features(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockFeaturesModel);
        }

        [Test]
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

        [Test]
        public static void Post_Features_NullModel_ThrowsException()
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.Features((FeaturesModel) null));
        }

        [Test]
        public static async Task Post_Features_InvalidModel_DoesNotCallService()
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.Features(new Mock<FeaturesModel>().Object);

            mockService.Verify(
                x => x.SaveSolutionFeatures(It.IsAny<string>(), It.IsAny<string[]>()), Times.Never);
        }

        [Test]
        public static async Task Post_Features_InvalidModel_ReturnsViewWithModel()
        {
            var mockFeaturesModel = new Mock<FeaturesModel>().Object;
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Features(mockFeaturesModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockFeaturesModel);
        }

        [Test]
        public static async Task Post_Features_ValidModel_MapsModelToArray()
        {
            var mockFeaturesModel = new Mock<FeaturesModel>().Object;
            var mockMapper = new Mock<IMapper>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                mockMapper.Object, Mock.Of<ISolutionsService>());

            await controller.Features(mockFeaturesModel);

            mockMapper.Verify(x => x.Map<FeaturesModel, string[]>(mockFeaturesModel));
        }

        [Test, AutoData]
        public static async Task Post_Features_ValidModel_CallsService(FeaturesModel model, string[] features)
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<FeaturesModel, string[]>(model))
                .Returns(features);
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                mockMapper.Object, mockService.Object);

            await controller.Features(model);

            mockService.Verify(x => x.SaveSolutionFeatures(model.SolutionId, features));
        }

        [Test, AutoData]
        public static async Task Post_Features_ValidModel_RedirectsToExpectedAction(FeaturesModel model,
            string[] features)
        {
            var mockMapper = new Mock<IMapper>();
            mockMapper.Setup(x => x.Map<FeaturesModel, string[]>(model))
                .Returns(features);
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.Features(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be("Solution");
            actual.RouteValues["id"].Should().Be(model.SolutionId);
        }

        [Test]
        public static void Get_ImplementationTimescales_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(AboutSolutionController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutSolutionController.ImplementationTimescales)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(AboutSolutionController.ImplementationTimescales).ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_ImplementationTimescales_InvalidId_ThrowsException(string id)
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.ImplementationTimescales(id));
        }

        [Test, AutoData]
        public static async Task Get_ImplementationTimescales_ValidId_GetsSolutionFromService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ImplementationTimescales(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_ImplementationTimescales_NullSolutionFromService_ReturnsBadRequestResponse(
            string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.ImplementationTimescales(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_ImplementationTimescales_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                mockMapper.Object, mockService.Object);

            await controller.ImplementationTimescales(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, ImplementationTimescalesModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_ImplementationTimescales_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockImplementationTimescalesModel = new Mock<ImplementationTimescalesModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, ImplementationTimescalesModel>(mockCatalogueItem))
                .Returns(mockImplementationTimescalesModel);
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.ImplementationTimescales(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockImplementationTimescalesModel);
        }

        [Test]
        public static void Post_ImplementationTimescales_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(AboutSolutionController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutSolutionController.ImplementationTimescales)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(AboutSolutionController.ImplementationTimescales).ToLowerCaseHyphenated());
        }

        [Test]
        public static void Post_ImplementationTimescales_NullModel_ThrowsException()
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() =>
                controller.ImplementationTimescales((ImplementationTimescalesModel) null));
        }

        [Test]
        public static async Task Post_ImplementationTimescales_InvalidModel_DoesNotCallService()
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.ImplementationTimescales(new Mock<ImplementationTimescalesModel>().Object);

            mockService.Verify(
                x => x.SaveImplementationDetail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public static async Task Post_ImplementationTimescales_InvalidModel_ReturnsViewWithModel()
        {
            var mockImplementationTimescalesModel = new Mock<ImplementationTimescalesModel>().Object;
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.ImplementationTimescales(mockImplementationTimescalesModel))
                .As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockImplementationTimescalesModel);
        }

        [Test, AutoData]
        public static async Task Post_ImplementationTimescales_ValidModel_CallsService(
            ImplementationTimescalesModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.ImplementationTimescales(model);

            mockService.Verify(x => x.SaveImplementationDetail(model.SolutionId, model.Description));
        }

        [Test, AutoData]
        public static async Task Post_ImplementationTimescales_ValidModel_RedirectsToExpectedAction(
            ImplementationTimescalesModel model)
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            var actual = (await controller.ImplementationTimescales(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be("Solution");
            actual.RouteValues["id"].Should().Be(model.SolutionId);
        }

        [Test]
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

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_Integrations_InvalidId_ThrowsException(string id)
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.Integrations(id));
        }

        [Test, AutoData]
        public static async Task Get_Integrations_ValidId_GetsSolutionFromService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Integrations(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_Integrations_NullSolutionFromService_ReturnsBadRequestResponse(
            string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.Integrations(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_Integrations_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                mockMapper.Object, mockService.Object);

            await controller.Integrations(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, IntegrationsModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_Integrations_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockIntegrationsModel = new Mock<IntegrationsModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, IntegrationsModel>(mockCatalogueItem))
                .Returns(mockIntegrationsModel);
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.Integrations(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockIntegrationsModel);
        }

        [Test]
        public static void Post_Integrations_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(AboutSolutionController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutSolutionController.Integrations)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(AboutSolutionController.Integrations).ToLowerCaseHyphenated());
        }

        [Test]
        public static void Post_Integrations_NullModel_ThrowsException()
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.Integrations((IntegrationsModel) null));
        }

        [Test]
        public static async Task Post_Integrations_InvalidModel_DoesNotCallService()
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.Integrations(new Mock<IntegrationsModel>().Object);

            mockService.Verify(
                x => x.SaveImplementationDetail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public static async Task Post_Integrations_InvalidModel_ReturnsViewWithModel()
        {
            var mockIntegrationsModel = new Mock<IntegrationsModel>().Object;
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Integrations(mockIntegrationsModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockIntegrationsModel);
        }

        [Test, AutoData]
        public static async Task Post_Integrations_ValidModel_CallsService(IntegrationsModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Integrations(model);

            mockService.Verify(x => x.SaveIntegrationLink(model.SolutionId, model.Link));
        }

        [Test, AutoData]
        public static async Task Post_Integrations_ValidModel_RedirectsToExpectedAction(
            IntegrationsModel model)
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            var actual = (await controller.Integrations(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be("Solution");
            actual.RouteValues["id"].Should().Be(model.SolutionId);
        }

        [Test]
        public static void Get_Roadmap_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(AboutSolutionController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutSolutionController.Roadmap)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(nameof(AboutSolutionController.Roadmap).ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_Roadmap_InvalidId_ThrowsException(string id)
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.Roadmap(id));
        }


        [Test, AutoData]
        public static async Task Get_Roadmap_ValidId_GetsSolutionFromService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Roadmap(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_Roadmap_NullSolutionFromService_ReturnsBadRequestResponse(
            string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.Roadmap(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_Roadmap_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                mockMapper.Object, mockService.Object);

            await controller.Roadmap(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, RoadmapModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_Roadmap_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockRoadmapModel = new Mock<RoadmapModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, RoadmapModel>(mockCatalogueItem))
                .Returns(mockRoadmapModel);
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.Roadmap(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockRoadmapModel);
        }

        [Test]
        public static void Post_Roadmap_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(AboutSolutionController)
                .GetMethods()
                .First(x => x.Name == nameof(AboutSolutionController.Roadmap)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(nameof(AboutSolutionController.Roadmap).ToLowerCaseHyphenated());
        }

        [Test]
        public static void Post_Roadmap_NullModel_ThrowsException()
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.Roadmap((RoadmapModel) null));
        }

        [Test]
        public static async Task Post_Roadmap_InvalidModel_DoesNotCallService()
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.Roadmap(new Mock<RoadmapModel>().Object);

            mockService.Verify(
                x => x.SaveImplementationDetail(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public static async Task Post_Roadmap_InvalidModel_ReturnsViewWithModel()
        {
            var mockRoadmapModel = new Mock<RoadmapModel>().Object;
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Roadmap(mockRoadmapModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockRoadmapModel);
        }

        [Test, AutoData]
        public static async Task Post_Roadmap_ValidModel_CallsService(RoadmapModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Roadmap(model);

            mockService.Verify(x => x.SaveRoadmap(model.SolutionId, model.Summary));
        }

        [Test, AutoData]
        public static async Task Post_Roadmap_ValidModel_RedirectsToExpectedAction(RoadmapModel model)
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            var actual = (await controller.Roadmap(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be("Solution");
            actual.RouteValues["id"].Should().Be(model.SolutionId);
        }

        [Test]
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

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_SolutionDescription_InvalidId_ThrowsException(string id)
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.SolutionDescription(id));
        }

        [Test, AutoData]
        public static async Task Get_SolutionDescription_ValidId_GetsSolutionFromService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.SolutionDescription(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_SolutionDescription_NullSolutionFromService_ReturnsBadRequestResponse(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.SolutionDescription(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_SolutionDescription_ValidId_MapsSolutionToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                mockMapper.Object, mockService.Object);

            await controller.SolutionDescription(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, SolutionDescriptionModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_SolutionDescription_ValidId_ReturnsViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var catalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(catalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockSolutionDescriptionModel = new Mock<SolutionDescriptionModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, SolutionDescriptionModel>(catalogueItem))
                .Returns(mockSolutionDescriptionModel);
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.SolutionDescription(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockSolutionDescriptionModel);
        }

        [Test]
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

        [Test]
        public static void Post_SolutionDescription_NullModel_ThrowsException()
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() =>
                controller.SolutionDescription((SolutionDescriptionModel) null));
        }

        [Test]
        public static async Task Post_SolutionDescription_InvalidModel_DoesNotCallService()
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);
            controller.ModelState.AddModelError("some-property", "some-error");

            await controller.SolutionDescription(new Mock<SolutionDescriptionModel>().Object);

            mockService.Verify(
                x => x.SaveSolutionDescription(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>()), Times.Never);
        }

        [Test]
        public static async Task Post_SolutionDescription_InvalidModel_ReturnsViewWithModel()
        {
            var mockSolutionDescriptionModel = new Mock<SolutionDescriptionModel>().Object;
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.SolutionDescription(mockSolutionDescriptionModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockSolutionDescriptionModel);
        }

        [Test, AutoData]
        public static async Task Post_SolutionDescription_ValidModel_CallsSaveSolutionDescriptionOnService(
            SolutionDescriptionModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.SolutionDescription(model);

            mockService.Verify(x => x.SaveSolutionDescription(model.SolutionId, model.Summary, model.Description,
                model.Link));
        }

        [Test, AutoData]
        public static async Task Post_SolutionDescription_ValidModel_RedirectsToExpectedAction(
            SolutionDescriptionModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.SolutionDescription(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be("Solution");
            actual.RouteValues["id"].Should().Be(model.SolutionId);
        }
    }
}