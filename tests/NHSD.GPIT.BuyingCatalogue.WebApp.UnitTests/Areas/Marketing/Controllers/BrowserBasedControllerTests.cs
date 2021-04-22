using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class BrowserBasedControllerTests
    {
        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(BrowserBasedController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new BrowserBasedController(null, Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), null));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_BrowserBasedSupportedBrowsers_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.SupportedBrowsers(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_BrowserBasedMobileFirstApproach_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.MobileFirstApproach(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_BrowserBasedPlugInsOrExtensions_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.PlugInsOrExtensions(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_BrowserBasedConnectivityAndResolution_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.ConnectivityAndResolution(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_BrowserBasedHardwareRequirements_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.HardwareRequirements(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_BrowserBasedAdditionalInformation_InvalidId_ThrowsException(string id)
        {
            var controller = new BrowserBasedController(Mock.Of<ILogWrapper<BrowserBasedController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.AdditionalInformation(id));
        }
    }
}
