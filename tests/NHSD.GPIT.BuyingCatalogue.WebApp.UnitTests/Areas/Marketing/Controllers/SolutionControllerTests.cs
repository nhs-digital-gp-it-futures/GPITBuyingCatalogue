using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SolutionControllerTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };

        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(SolutionController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new SolutionController(null, Mock.Of<IMapper>(), Mock.Of<ISolutionsService>()))
                .ParamName.Should().Be("logger");
        }

        [Test]
        public static void Constructor_NullMapper_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), null,
                    Mock.Of<ISolutionsService>()))
                .ParamName.Should().Be("mapper");
        }

        [Test]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), Mock.Of<IMapper>(), null))
                .ParamName.Should().Be("solutionsService");
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_Index_InvalidId_ThrowsException(string id)
        {
            var controller = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), Mock.Of<IMapper>(),
                Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.Index(id));
        }

        [Test, AutoData]
        public static async Task Get_Index_ValidId_GetsSolutionFromService(string id)
        {
            var mockService = new Mock<ISolutionsService>();
            var controller = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), Mock.Of<IMapper>(),
                mockService.Object);

            await controller.Index(id);

            mockService.Verify(s => s.GetSolution(id));
        }

        [Test, AutoData]
        public static async Task Get_Index_NullSolutionForId_ReturnsBadRequestResult(string id)
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

        [Test, AutoData]
        public static async Task Get_Index_ValidSolutionForId_MapsToModel(string id)
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

        [Test, AutoData]
        public static async Task Get_Index_ValidSolutionForId_ReturnsExpectedViewResult(string id)
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

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_Preview_InvalidId_ThrowsException(string id)
        {
            var controller = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), Mock.Of<IMapper>(),
                Mock.Of<ISolutionsService>());

            Assert.Throws<ArgumentException>(() => controller.Preview(id));
        }

        [Test]
        public static void Get_Preview_RedirectsToPreview()
        {
            var controller = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), Mock.Of<IMapper>(),
                Mock.Of<ISolutionsService>());

            var result = (controller.Preview("123")).As<RedirectToActionResult>();

            result.Should().NotBeNull();
            result.ActionName.Should().Be(nameof(SolutionDetailsController.PreviewSolutionDetail));
            result.ControllerName.Should().Be(typeof(SolutionDetailsController).ControllerName());
            result.RouteValues["id"].Should().Be("123");
        }
    }
}
