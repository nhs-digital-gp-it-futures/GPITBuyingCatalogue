using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    public static class SolutionControllerTests
    {
        [Fact]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(SolutionController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
        }

        [Fact]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new SolutionController(null, Mock.Of<IMapper>(), Mock.Of<ISolutionsService>()))
                .ParamName.Should().Be("logger");
        }

        [Fact]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), null,
                    Mock.Of<ISolutionsService>()))
                .ParamName.Should().Be("mapper");
        }

        [Fact]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), Mock.Of<IMapper>(), null))
                .ParamName.Should().Be("solutionsService");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ValidId_GetsSolutionFromService(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), Mock.Of<IMapper>(),
                mockService.Object);

            await controller.Index(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_NullSolutionForId_ReturnsBadRequestResult(CatalogueItemId id)
        {
            var mockService = new Mock<ISolutionsService>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(default(CatalogueItem));
            var controller = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), Mock.Of<IMapper>(),
                mockService.Object);

            var actual = (await controller.Index(id)).As<BadRequestObjectResult>();

            actual.Should().NotBeNull();
            actual.Value.Should().Be($"No Catalogue Item found for Id: {id}");
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ValidSolutionForId_MapsToModel(CatalogueItemId id)
        {
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;
            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            var controller = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), mockMapper.Object,
                mockService.Object);

            await controller.Index(id);

            mockMapper.Verify(m => m.Map<CatalogueItem, SolutionStatusModel>(mockCatalogueItem));
        }

        [Theory]
        [CommonAutoData]
        public static async Task Get_Index_ValidSolutionForId_ReturnsExpectedViewResult(CatalogueItemId id)
        {
            var mockSolutionStatusModel = new Mock<SolutionStatusModel>().Object;
            var mockCatalogueItem = new Mock<CatalogueItem>().Object;

            var mockService = new Mock<ISolutionsService>();
            var mockMapper = new Mock<IMapper>();
            mockService.Setup(s => s.GetSolution(id))
                .ReturnsAsync(mockCatalogueItem);
            mockMapper.Setup(m => m.Map<CatalogueItem, SolutionStatusModel>(mockCatalogueItem))
                .Returns(mockSolutionStatusModel);
            var controller = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), mockMapper.Object,
                mockService.Object);

            var actual = (await controller.Index(id)).As<ViewResult>();

            actual.Should().NotBeNull();
            actual.ViewName.Should().BeNullOrEmpty();
            actual.Model.Should().Be(mockSolutionStatusModel);
        }

        [Theory]
        [CommonAutoData]
        public static void Get_Preview_RedirectsToPreview(CatalogueItemId id)
        {
            var controller = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), Mock.Of<IMapper>(),
                Mock.Of<ISolutionsService>());

            var result = controller.Preview(id).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(SolutionDetailsController.PreviewSolutionDetail));
            result.ControllerName.Should().Be(typeof(SolutionDetailsController).ControllerName());
            result.RouteValues["solutionId"].Should().Be(id);
        }
    }
}
