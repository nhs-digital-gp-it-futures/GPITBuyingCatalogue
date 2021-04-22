using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SolutionControllerTests
    {
        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(SolutionController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new SolutionController(null, Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), null));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_Index_InvalidId_ThrowsException(string id)
        {
            var controller = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.Index(id));
        }

        [Test]
        public static async Task Get_Index_EmptySolution_AllIncomplete()
        {
            var mockService = new Mock<ISolutionsService>();

            mockService.Setup(x => x.GetSolution(It.IsAny<string>())).ReturnsAsync(new CatalogueItem { Solution = new Solution(), Supplier = new Supplier() });

            var controller = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), mockService.Object);

            var result = await controller.Index("123");

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
            Assert.That(((ViewResult)result).Model, Is.InstanceOf(typeof(SolutionStatusModel)));

            var model = (SolutionStatusModel)((ViewResult)result).Model;

            // MJRTODO - The rest
            Assert.AreEqual("INCOMPLETE", model.ContactDetailsStatus);
            Assert.AreEqual("INCOMPLETE", model.ClientApplicationTypeStatus);
            Assert.AreEqual("INCOMPLETE", model.FeaturesStatus);
            Assert.AreEqual("INCOMPLETE", model.ImplementationTimescalesStatus);
            Assert.AreEqual("INCOMPLETE", model.IntegrationsStatus);
            Assert.AreEqual("INCOMPLETE", model.RoadmapStatus);
            Assert.AreEqual("INCOMPLETE", model.SolutionDescriptionStatus);            
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_Preview_InvalidId_ThrowsException(string id)
        {
            var controller = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), Mock.Of<ISolutionsService>());

            Assert.Throws<ArgumentException>(() => controller.Preview(id));
        }

        [Test]
        public static void Get_Preview_RedirectsToPreview()
        {
            var controller = new SolutionController(Mock.Of<ILogWrapper<SolutionController>>(), Mock.Of<ISolutionsService>());

            var result = controller.Preview("123");

            Assert.That(result, Is.InstanceOf(typeof(RedirectToActionResult)));
            Assert.AreEqual("preview", ((RedirectToActionResult)result).ActionName);
            Assert.AreEqual("solutions", ((RedirectToActionResult)result).ControllerName);
            Assert.AreEqual("123", ((RedirectToActionResult)result).RouteValues["id"]);
        }
    }
}
