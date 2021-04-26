using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    internal static class NativeDesktopControllerTests
    {
        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(NativeDesktopController).Should()
                .BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
            typeof(NativeDesktopController).Should()
                .BeDecoratedWith<RouteAttribute>(x => x.Template == "marketing/supplier/solution/{id}/section/native-desktop");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new NativeDesktopController(null, Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(), null));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeDesktopOperatingSystems_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.OperatingSystems(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeDesktopConnectivity_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.Connectivity(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeDesktopMemoryAndStorage_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.MemoryAndStorage(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeDesktopThirdParty_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.ThirdParty(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeDesktopHardwareRequirements_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.HardwareRequirements(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeDesktopAdditionalInformation_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeDesktopController(Mock.Of<ILogWrapper<NativeDesktopController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.AdditionalInformation(id));
        }
    }
}
