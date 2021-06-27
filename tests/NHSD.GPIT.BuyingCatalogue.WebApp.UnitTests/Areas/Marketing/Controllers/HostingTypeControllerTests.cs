using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    public static class HostingTypeControllerTests
    {
        [Fact]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new HostingTypeController( null,
                    Mock.Of<ISolutionsService>())).ParamName.Should().Be("mapper");
        }

        [Fact]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new HostingTypeController( Mock.Of<IMapper>(), null))
                .ParamName.Should().Be("solutionsService");
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_PublicCloud_ValidId_CallsGetSolutionOnService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PublicCloud(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PublicCloud_NullSolutionFromService_ReturnsBadRequestResponse(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PublicCloud(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PublicCloud_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new HostingTypeController(
                mockMapper.Object, mockService.Object);

            await controller.PublicCloud(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, PublicCloudModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PublicCloud_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockHostingTypePublicCloudModel = new Mock<PublicCloudModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, PublicCloudModel>(mockCatalogueItem))
                .Returns(mockHostingTypePublicCloudModel);
            var controller = new HostingTypeController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.PublicCloud(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockHostingTypePublicCloudModel);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_PublicCloud_InvalidModel_ReturnsViewWithModel(CatalogueItemId id)
        {
            var mockPublicCloudModel = new Mock<PublicCloudModel>().Object;
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.PublicCloud(id, mockPublicCloudModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockPublicCloudModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PublicCloud_ValidModel_GetsHostingFromService(
            [Frozen] CatalogueItemId id,
            PublicCloudModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PublicCloud(id, model);

            mockService.Verify(s => s.GetHosting(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PublicCloud_NoHostingFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            PublicCloudModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetHosting(id))
                .ReturnsAsync(default(Hosting));
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PublicCloud(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Hosting found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PublicCloud_ValidModel_MapsModelToHosting(
            [Frozen] CatalogueItemId id,
            PublicCloudModel model)
        {
            var mockMapper = new Mock<IMapper>();
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>();
            mockService.Setup(s => s.GetHosting(id))
                .ReturnsAsync(mockHosting.Object);
            var controller = new HostingTypeController(
                mockMapper.Object, mockService.Object);

            await controller.PublicCloud(id, model);

            mockHosting.VerifySet(h => h.PublicCloud = model.PublicCloud);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PublicCloud_ValidModel_CallSaveClientApplicationOnService(
            [Frozen] CatalogueItemId id,
            PublicCloudModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>().Object;
            mockService.Setup(s => s.GetHosting(id))
                .ReturnsAsync(mockHosting);
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PublicCloud(id, model);

            mockService.Verify(s => s.SaveHosting(id, mockHosting));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PublicCloud_ValidModel_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            PublicCloudModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>().Object;
            mockService.Setup(s => s.GetHosting(id))
                .ReturnsAsync(mockHosting);
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PublicCloud(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be(nameof(SolutionController).ToControllerName());
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_PrivateCloud_ValidId_CallsGetSolutionOnService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PrivateCloud(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PrivateCloud_NullSolutionFromService_ReturnsBadRequestResponse(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PrivateCloud(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PrivateCloud_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new HostingTypeController(
                mockMapper.Object, mockService.Object);

            await controller.PrivateCloud(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, PrivateCloudModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_PrivateCloud_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockHostingTypePrivateCloudModel = new Mock<PrivateCloudModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, PrivateCloudModel>(mockCatalogueItem))
                .Returns(mockHostingTypePrivateCloudModel);
            var controller = new HostingTypeController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.PrivateCloud(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockHostingTypePrivateCloudModel);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_PrivateCloud_InvalidModel_ReturnsViewWithModel([Frozen] CatalogueItemId id)
        {
            var mockPrivateCloudModel = new Mock<PrivateCloudModel>().Object;
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.PrivateCloud(id, mockPrivateCloudModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockPrivateCloudModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PrivateCloud_ValidModel_GetsHostingFromService(
            [Frozen] CatalogueItemId id,
            PrivateCloudModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PrivateCloud(id, model);

            mockService.Verify(s => s.GetHosting(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PrivateCloud_NoHostingFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            PrivateCloudModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetHosting(id)).ReturnsAsync(default(Hosting));
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PrivateCloud(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Hosting found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PrivateCloud_ValidModel_MapsModelToHosting(
            [Frozen] CatalogueItemId id,
            PrivateCloudModel model)
        {
            var mockMapper = new Mock<IMapper>();
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>();
            mockService.Setup(s => s.GetHosting(id)).ReturnsAsync(mockHosting.Object);

            var controller = new HostingTypeController(
                mockMapper.Object, mockService.Object);

            await controller.PrivateCloud(id, model);

            mockHosting.VerifySet(h => h.PrivateCloud = model.PrivateCloud);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PrivateCloud_ValidModel_CallSaveClientApplicationOnService(
            [Frozen] CatalogueItemId id,
            PrivateCloudModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>().Object;
            mockService.Setup(s => s.GetHosting(id))
                .ReturnsAsync(mockHosting);
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.PrivateCloud(id, model);

            mockService.Verify(s => s.SaveHosting(id, mockHosting));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_PrivateCloud_ValidModel_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            PrivateCloudModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>().Object;
            mockService.Setup(s => s.GetHosting(id))
                .ReturnsAsync(mockHosting);
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.PrivateCloud(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be(nameof(SolutionController).ToControllerName());
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_Hybrid_ValidId_CallsGetSolutionOnService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Hybrid(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Hybrid_NullSolutionFromService_ReturnsBadRequestResponse(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.Hybrid(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Hybrid_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new HostingTypeController(
                mockMapper.Object, mockService.Object);

            await controller.Hybrid(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, HybridModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Hybrid_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockHybridModel = new Mock<HybridModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, HybridModel>(mockCatalogueItem))
                .Returns(mockHybridModel);
            var controller = new HostingTypeController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.Hybrid(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockHybridModel);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_Hybrid_InvalidModel_ReturnsViewWithModel([Frozen] CatalogueItemId id)
        {
            var mockHybridModel = new Mock<HybridModel>().Object;
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.Hybrid(id, mockHybridModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockHybridModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Hybrid_ValidModel_GetsHostingFromService(
            [Frozen] CatalogueItemId id,
            HybridModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Hybrid(id, model);

            mockService.Verify(s => s.GetHosting(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Hybrid_NoHostingFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            HybridModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetHosting(id))
                .ReturnsAsync(default(Hosting));
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.Hybrid(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Hosting found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Hybrid_ValidModel_MapsModelToHosting(
            [Frozen] CatalogueItemId id,
            HybridModel model)
        {
            var mockMapper = new Mock<IMapper>();
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>();
            mockService.Setup(s => s.GetHosting(id))
                .ReturnsAsync(mockHosting.Object);
            var controller = new HostingTypeController(
                mockMapper.Object, mockService.Object);

            await controller.Hybrid(id, model);

            mockHosting.VerifySet(h => h.HybridHostingType = model.HybridHostingType);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Hybrid_ValidModel_CallSaveClientApplicationOnService(
            [Frozen] CatalogueItemId id,
            HybridModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>().Object;
            mockService.Setup(s => s.GetHosting(id))
                .ReturnsAsync(mockHosting);
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.Hybrid(id, model);

            mockService.Verify(s => s.SaveHosting(id, mockHosting));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_Hybrid_ValidModel_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            HybridModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>().Object;
            mockService.Setup(s => s.GetHosting(id)).ReturnsAsync(mockHosting);

            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.Hybrid(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be(nameof(SolutionController).ToControllerName());
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Get_OnPremise_ValidId_CallsGetSolutionOnService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.OnPremise(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OnPremise_NullSolutionFromService_ReturnsBadRequestResponse(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.OnPremise(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OnPremise_ValidSolutionFromService_MapsToModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var controller = new HostingTypeController(
                mockMapper.Object, mockService.Object);

            await controller.OnPremise(id);

            mockMapper.Verify(x => x.Map<CatalogueItem, OnPremiseModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_OnPremise_ValidId_ReturnsExpectedViewWithModel(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var mockMapper = new Mock<IMapper>();
            var mockOnPremiseModel = new Mock<OnPremiseModel>().Object;
            mockMapper.Setup(x => x.Map<CatalogueItem, OnPremiseModel>(mockCatalogueItem))
                .Returns(mockOnPremiseModel);
            var controller = new HostingTypeController(
                mockMapper.Object, mockService.Object);

            var actual = (await controller.OnPremise(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockOnPremiseModel);
        }

        [Fact]
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

        [Theory]
        [CommonAutoData]
        public static async Task Post_OnPremise_InvalidModel_ReturnsViewWithModel([Frozen] CatalogueItemId id)
        {
            var mockOnPremiseModel = new Mock<OnPremiseModel>().Object;
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());
            controller.ModelState.AddModelError("some-property", "some-error");

            var actual = (await controller.OnPremise(id, mockOnPremiseModel)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNull();
            actual.Model.Should().Be(mockOnPremiseModel);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OnPremise_ValidModel_GetsHostingFromService(
            [Frozen] CatalogueItemId id,
            OnPremiseModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.OnPremise(id, model);

            mockService.Verify(s => s.GetHosting(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OnPremise_NoHostingFromService_ReturnsBadRequestResult(
            [Frozen] CatalogueItemId id,
            OnPremiseModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetHosting(id))
                .ReturnsAsync(default(Hosting));
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.OnPremise(id, model)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Hosting found for Solution Id: {model.SolutionId}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OnPremise_ValidModel_MapsModelToHosting(
            [Frozen] CatalogueItemId id,
            OnPremiseModel model)
        {
            var mockMapper = new Mock<IMapper>();
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>();
            mockService.Setup(s => s.GetHosting(id))
                .ReturnsAsync(mockHosting.Object);
            var controller = new HostingTypeController(
                mockMapper.Object, mockService.Object);

            await controller.OnPremise(id, model);

            mockHosting.VerifySet(h => h.OnPremise = model.OnPremise);
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OnPremise_ValidModel_CallSaveClientApplicationOnService(
            [Frozen] CatalogueItemId id,
            OnPremiseModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>().Object;
            mockService.Setup(s => s.GetHosting(id))
                .ReturnsAsync(mockHosting);
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            await controller.OnPremise(id, model);

            mockService.Verify(s => s.SaveHosting(id, mockHosting));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Post_OnPremise_ValidModel_ReturnsRedirectResult(
            [Frozen] CatalogueItemId id,
            OnPremiseModel model)
        {
            var mockService = new Mock<ISolutionsService>();
            var mockHosting = new Mock<Hosting>().Object;
            mockService.Setup(s => s.GetHosting(id))
                .ReturnsAsync(mockHosting);
            var controller = new HostingTypeController(
                Mock.Of<IMapper>(), mockService.Object);

            var actual = (await controller.OnPremise(id, model)).As<RedirectToActionResult>();

            actual.Should().NotBeNull();
            actual.ActionName.Should().Be(nameof(SolutionController.Index));
            actual.ControllerName.Should().Be(nameof(SolutionController).ToControllerName());
        }
    }
}
