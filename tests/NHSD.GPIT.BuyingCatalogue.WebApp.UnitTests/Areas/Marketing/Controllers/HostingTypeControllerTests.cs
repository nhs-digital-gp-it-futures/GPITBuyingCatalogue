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
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class HostingTypeControllerTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };

        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(HostingTypeController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
            typeof(HostingTypeController).Should()
                .BeDecoratedWith<RouteAttribute>(x => x.Template == "marketing/supplier/solution/{id}/section");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new HostingTypeController(null, Mock.Of<IMapper>(), Mock.Of<ISolutionsService>()))
                .ParamName.Should().Be("logger");
        }

        [Test]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(), null,
                    Mock.Of<ISolutionsService>())).ParamName.Should().Be("mapper");
        }

        [Test]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(), Mock.Of<IMapper>(), null))
                .ParamName.Should().Be("solutionsService");
        }

        [Test]
        public static void Get_PublicCloud_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(HostingTypeController)
                .GetMethods()
                .First(x => x.Name == nameof(HostingTypeController.PublicCloud)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(
                    $"{nameof(HostingTypeController).ToControllerName()}{nameof(HostingTypeController.PublicCloud)}"
                        .ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_PublicCloud_InvalidId_ThrowsException(string id)
        {
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.PublicCloud(id))
                .Message.Should().Be($"hosting-type-public-cloud-{nameof(id)}");
        }

        [Test, AutoData]
        public static async Task Get_PublicCloud_ValidId_CallsGetSolutionOnService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PublicCloud(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_PublicCloud_NullSolutionFromService_ReturnsBadRequestResponse(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PublicCloud(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_PublicCloud_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                mockMapper.Object, mockService.Object);

            await controller.PublicCloud(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, PublicCloudModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_PublicCloud_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockHostingTypePublicCloudModel = new Mock<PublicCloudModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, PublicCloudModel>(mockCatalogueItem))
                .Returns(mockHostingTypePublicCloudModel);
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.PublicCloud(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockHostingTypePublicCloudModel);
        }

        [Test]
        public static void Post_PublicCloud_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(HostingTypeController)
                .GetMethods()
                .First(x => x.Name == nameof(HostingTypeController.PublicCloud)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(
                    $"{nameof(HostingTypeController).ToControllerName()}{nameof(HostingTypeController.PublicCloud)}"
                        .ToLowerCaseHyphenated());
        }

        [Test]
        public static void Post_PublicCloud_NullModel_ThrowsException()
        {
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() =>
                controller.PublicCloud(default(PublicCloudModel)))
                .ParamName.Should().Be("model");
        }

        [Test]
        public static async Task Post_PublicCloud_InvalidModel_ReturnsViewWithModel()
        {
            var mockPublicCloudModel = new Mock<PublicCloudModel>().Object;
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.PublicCloud(mockPublicCloudModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockPublicCloudModel);
        }

        [Test, AutoData]
        public static async Task Post_PublicCloud_ValidModel_GetsHostingFromService(
            PublicCloudModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PublicCloud(model);

            mockService.Verify(x => x.GetHosting(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_PublicCloud_NoHostingFromService_ReturnsBadRequestResult(
            PublicCloudModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetHosting(model.SolutionId))
                .ReturnsAsync(default(Hosting));
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PublicCloud(model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Hosting found for Solution Id: {model.SolutionId}");
        }

        [Test, AutoData]
        public static async Task Post_PublicCloud_ValidModel_MapsModelToHosting(
            PublicCloudModel model)
        {
            var mockMapper = new Mock<IMapper>();
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>();
            mockService.Setup(x => x.GetHosting(model.SolutionId))
                .ReturnsAsync(mockHosting.Object);
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                mockMapper.Object, mockService.Object);

            await controller.PublicCloud(model);

            mockHosting.VerifySet(h => h.PublicCloud = model.PublicCloud);
        }

        [Test, AutoData]
        public static async Task Post_PublicCloud_ValidModel_CallSaveClientApplicationOnService(
            PublicCloudModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>().Object;
            mockService.Setup(x => x.GetHosting(model.SolutionId))
                .ReturnsAsync(mockHosting);
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PublicCloud(model);

            mockService.Verify(x => x.SaveHosting(model.SolutionId, mockHosting));
        }

        [Test, AutoData]
        public static async Task Post_PublicCloud_ValidModel_ReturnsRedirectResult(
            PublicCloudModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>().Object;
            mockService.Setup(x => x.GetHosting(model.SolutionId))
                .ReturnsAsync(mockHosting);
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PublicCloud(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be(nameof(SolutionController).ToControllerName());
        }

        [Test]
        public static void Get_PrivateCloud_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(HostingTypeController)
                .GetMethods()
                .First(x => x.Name == nameof(HostingTypeController.PrivateCloud)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(
                    $"{nameof(HostingTypeController).ToControllerName()}{nameof(HostingTypeController.PrivateCloud)}"
                        .ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_PrivateCloud_InvalidId_ThrowsException(string id)
        {
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.PrivateCloud(id))
                .Message.Should().Be($"hosting-type-private-cloud-{nameof(id)}");
        }

        [Test, AutoData]
        public static async Task Get_PrivateCloud_ValidId_CallsGetSolutionOnService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PrivateCloud(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_PrivateCloud_NullSolutionFromService_ReturnsBadRequestResponse(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PrivateCloud(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_PrivateCloud_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                mockMapper.Object, mockService.Object);

            await controller.PrivateCloud(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, PrivateCloudModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_PrivateCloud_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockHostingTypePrivateCloudModel = new Mock<PrivateCloudModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, PrivateCloudModel>(mockCatalogueItem))
                .Returns(mockHostingTypePrivateCloudModel);
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.PrivateCloud(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockHostingTypePrivateCloudModel);
        }

        [Test]
        public static void Post_PrivateCloud_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(HostingTypeController)
                .GetMethods()
                .First(x => x.Name == nameof(HostingTypeController.PrivateCloud)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(
                    $"{nameof(HostingTypeController).ToControllerName()}{nameof(HostingTypeController.PrivateCloud)}"
                        .ToLowerCaseHyphenated());
        }

        [Test]
        public static void Post_PrivateCloud_NullModel_ThrowsException()
        {
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() =>
                controller.PrivateCloud(default(PrivateCloudModel)))
                .ParamName.Should().Be("model");
        }

        [Test]
        public static async Task Post_PrivateCloud_InvalidModel_ReturnsViewWithModel()
        {
            var mockPrivateCloudModel = new Mock<PrivateCloudModel>().Object;
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.PrivateCloud(mockPrivateCloudModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockPrivateCloudModel);
        }

        [Test, AutoData]
        public static async Task Post_PrivateCloud_ValidModel_GetsHostingFromService(
            PrivateCloudModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PrivateCloud(model);

            mockService.Verify(x => x.GetHosting(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_PrivateCloud_NoHostingFromService_ReturnsBadRequestResult(
            PrivateCloudModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetHosting(model.SolutionId))
                .ReturnsAsync(default(Hosting));
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PrivateCloud(model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Hosting found for Solution Id: {model.SolutionId}");
        }

        [Test, AutoData]
        public static async Task Post_PrivateCloud_ValidModel_MapsModelToHosting(
            PrivateCloudModel model)
        {
            var mockMapper = new Mock<IMapper>();
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>();
            mockService.Setup(x => x.GetHosting(model.SolutionId))
                .ReturnsAsync(mockHosting.Object);
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                mockMapper.Object, mockService.Object);

            await controller.PrivateCloud(model);

            mockHosting.VerifySet(h => h.PrivateCloud = model.PrivateCloud);
        }

        [Test, AutoData]
        public static async Task Post_PrivateCloud_ValidModel_CallSaveClientApplicationOnService(
            PrivateCloudModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>().Object;
            mockService.Setup(x => x.GetHosting(model.SolutionId))
                .ReturnsAsync(mockHosting);
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PrivateCloud(model);

            mockService.Verify(x => x.SaveHosting(model.SolutionId, mockHosting));
        }

        [Test, AutoData]
        public static async Task Post_PrivateCloud_ValidModel_ReturnsRedirectResult(
            PrivateCloudModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>().Object;
            mockService.Setup(x => x.GetHosting(model.SolutionId))
                .ReturnsAsync(mockHosting);
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PrivateCloud(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be(nameof(SolutionController).ToControllerName());
        }

        [Test]
        public static void Get_Hybrid_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(HostingTypeController)
                .GetMethods()
                .First(x => x.Name == nameof(HostingTypeController.Hybrid)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(
                    $"{nameof(HostingTypeController).ToControllerName()}{nameof(HostingTypeController.Hybrid)}"
                        .ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_Hybrid_InvalidId_ThrowsException(string id)
        {
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.Hybrid(id))
                .Message.Should().Be($"hosting-type-hybrid-{nameof(id)}");
        }

        [Test, AutoData]
        public static async Task Get_Hybrid_ValidId_CallsGetSolutionOnService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Hybrid(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_Hybrid_NullSolutionFromService_ReturnsBadRequestResponse(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.Hybrid(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_Hybrid_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                mockMapper.Object, mockService.Object);

            await controller.Hybrid(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, HybridModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_Hybrid_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockHybridModel = new Mock<HybridModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, HybridModel>(mockCatalogueItem))
                .Returns(mockHybridModel);
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.Hybrid(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockHybridModel);
        }

        [Test]
        public static void Post_Hybrid_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(HostingTypeController)
                .GetMethods()
                .First(x => x.Name == nameof(HostingTypeController.Hybrid)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(
                    $"{nameof(HostingTypeController).ToControllerName()}{nameof(HostingTypeController.Hybrid)}"
                        .ToLowerCaseHyphenated());
        }

        [Test]
        public static void Post_Hybrid_NullModel_ThrowsException()
        {
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() =>
                controller.Hybrid(default(HybridModel)))
                .ParamName.Should().Be("model");
        }

        [Test]
        public static async Task Post_Hybrid_InvalidModel_ReturnsViewWithModel()
        {
            var mockHybridModel = new Mock<HybridModel>().Object;
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Hybrid(mockHybridModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockHybridModel);
        }

        [Test, AutoData]
        public static async Task Post_Hybrid_ValidModel_GetsHostingFromService(
            HybridModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Hybrid(model);

            mockService.Verify(x => x.GetHosting(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_Hybrid_NoHostingFromService_ReturnsBadRequestResult(
            HybridModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetHosting(model.SolutionId))
                .ReturnsAsync(default(Hosting));
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.Hybrid(model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Hosting found for Solution Id: {model.SolutionId}");
        }

        [Test, AutoData]
        public static async Task Post_Hybrid_ValidModel_MapsModelToHosting(
            HybridModel model)
        {
            var mockMapper = new Mock<IMapper>();
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>();
            mockService.Setup(x => x.GetHosting(model.SolutionId))
                .ReturnsAsync(mockHosting.Object);
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                mockMapper.Object, mockService.Object);

            await controller.Hybrid(model);

            mockHosting.VerifySet(h => h.HybridHostingType = model.HybridHostingType);
        }

        [Test, AutoData]
        public static async Task Post_Hybrid_ValidModel_CallSaveClientApplicationOnService(
            HybridModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>().Object;
            mockService.Setup(x => x.GetHosting(model.SolutionId))
                .ReturnsAsync(mockHosting);
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Hybrid(model);

            mockService.Verify(x => x.SaveHosting(model.SolutionId, mockHosting));
        }

        [Test, AutoData]
        public static async Task Post_Hybrid_ValidModel_ReturnsRedirectResult(
            HybridModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>().Object;
            mockService.Setup(x => x.GetHosting(model.SolutionId))
                .ReturnsAsync(mockHosting);
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.Hybrid(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be(nameof(SolutionController).ToControllerName());
        }

        [Test]
        public static void Get_OnPremise_HttpGetAttribute_ExpectedTemplate()
        {
            typeof(HostingTypeController)
                .GetMethods()
                .First(x => x.Name == nameof(HostingTypeController.OnPremise)
                            && x.GetCustomAttribute<HttpGetAttribute>() != null)
                .GetCustomAttribute<HttpGetAttribute>()
                .Template
                .Should().Be(
                    $"{nameof(HostingTypeController).ToControllerName()}{nameof(HostingTypeController.OnPremise)}"
                        .ToLowerCaseHyphenated());
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_OnPremise_InvalidId_ThrowsException(string id)
        {
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.OnPremise(id))
                .Message.Should().Be($"hosting-type-onpremise-{nameof(id)}");
        }

        [Test, AutoData]
        public static async Task Get_OnPremise_ValidId_CallsGetSolutionOnService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.OnPremise(id);

            mockService.Verify(x => x.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_OnPremise_NullSolutionFromService_ReturnsBadRequestResponse(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.OnPremise(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Test, AutoData]
        public static async Task Get_OnPremise_ValidSolutionFromService_MapsToModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                mockMapper.Object, mockService.Object);

            await controller.OnPremise(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, OnPremiseModel>(mockCatalogueItem));
        }

        [Test, AutoData]
        public static async Task Get_OnPremise_ValidId_ReturnsExpectedViewWithModel(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(x => x.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockOnPremiseModel = new Mock<OnPremiseModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, OnPremiseModel>(mockCatalogueItem))
                .Returns(mockOnPremiseModel);
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                mockMapper.Object, mockService.Object);

            var actual = (await controller.OnPremise(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockOnPremiseModel);
        }

        [Test]
        public static void Post_OnPremise_HttpPostAttribute_ExpectedTemplate()
        {
            typeof(HostingTypeController)
                .GetMethods()
                .First(x => x.Name == nameof(HostingTypeController.OnPremise)
                            && x.GetCustomAttribute<HttpPostAttribute>() != null)
                .GetCustomAttribute<HttpPostAttribute>()
                .Template
                .Should().Be(
                    $"{nameof(HostingTypeController).ToControllerName()}{nameof(HostingTypeController.OnPremise)}"
                        .ToLowerCaseHyphenated());
        }

        [Test]
        public static void Post_OnPremise_NullModel_ThrowsException()
        {
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() =>
                controller.OnPremise(default(OnPremiseModel)))
                .ParamName.Should().Be("model");
        }

        [Test]
        public static async Task Post_OnPremise_InvalidModel_ReturnsViewWithModel()
        {
            var mockOnPremiseModel = new Mock<OnPremiseModel>().Object;
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.OnPremise(mockOnPremiseModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockOnPremiseModel);
        }

        [Test, AutoData]
        public static async Task Post_OnPremise_ValidModel_GetsHostingFromService(
            OnPremiseModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.OnPremise(model);

            mockService.Verify(x => x.GetHosting(model.SolutionId));
        }

        [Test, AutoData]
        public static async Task Post_OnPremise_NoHostingFromService_ReturnsBadRequestResult(
            OnPremiseModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(x => x.GetHosting(model.SolutionId))
                .ReturnsAsync(default(Hosting));
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.OnPremise(model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Hosting found for Solution Id: {model.SolutionId}");
        }

        [Test, AutoData]
        public static async Task Post_OnPremise_ValidModel_MapsModelToHosting(
            OnPremiseModel model)
        {
            var mockMapper = new Mock<IMapper>();
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>();
            mockService.Setup(x => x.GetHosting(model.SolutionId))
                .ReturnsAsync(mockHosting.Object);
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                mockMapper.Object, mockService.Object);

            await controller.OnPremise(model);

            mockHosting.VerifySet(h => h.OnPremise = model.OnPremise);
        }

        [Test, AutoData]
        public static async Task Post_OnPremise_ValidModel_CallSaveClientApplicationOnService(
            OnPremiseModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>().Object;
            mockService.Setup(x => x.GetHosting(model.SolutionId))
                .ReturnsAsync(mockHosting);
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            await controller.OnPremise(model);

            mockService.Verify(x => x.SaveHosting(model.SolutionId, mockHosting));
        }

        [Test, AutoData]
        public static async Task Post_OnPremise_ValidModel_ReturnsRedirectResult(
            OnPremiseModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>().Object;
            mockService.Setup(x => x.GetHosting(model.SolutionId))
                .ReturnsAsync(mockHosting);
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.OnPremise(model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be(nameof(SolutionController).ToControllerName());
        }
    }
}
