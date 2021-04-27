using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutSolution;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class AboutSolutionControllerTests
    {
        private static string[] InvalidStrings = { null, string.Empty, "    " };

        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AboutSolutionController).Should()
                .BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AboutSolutionController(null, Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(), null));
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_SolutionDescription_InvalidId_ThrowsException(string id)
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.SolutionDescription(id));
        }

        [Test]

        public static void Post_SolutionDescription_NullModel_ThrowsException()
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.SolutionDescription((SolutionDescriptionModel)null));
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_Features_InvalidId_ThrowsException(string id)
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.Features(id));
        }

        [Test]
        public static void Post_Features_NullModel_ThrowsException()
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.Features((FeaturesModel)null));
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_Integrations_InvalidId_ThrowsException(string id)
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.Integrations(id));
        }

        [Test]
        public static void Post_Integrations_NullModel_ThrowsException()
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.Integrations((IntegrationsModel)null));
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_ImplementationTimescales_InvalidId_ThrowsException(string id)
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.ImplementationTimescales(id));
        }

        [Test]
        public static void Post_ImplementationTimescales_NullModel_ThrowsException()
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.ImplementationTimescales((ImplementationTimescalesModel)null));
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_Roadmap_InvalidId_ThrowsException(string id)
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.Roadmap(id));
        }

        [Test]
        public static void Post_Roadmap_NullModel_ThrowsException()
        {
            var controller = new AboutSolutionController(Mock.Of<ILogWrapper<AboutSolutionController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.Roadmap((RoadmapModel)null));
        }
    }
}
