using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class NativeMobileControllerTests
    {
        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(NativeMobileController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new NativeMobileController(null, Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new NativeMobileController(Mock.Of<ILogger<NativeMobileController>>(), null));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeMobileOperatingSystems_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogger<NativeMobileController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.NativeMobileOperatingSystems(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeMobileMobileFirstApproach_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogger<NativeMobileController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.NativeMobileMobileFirstApproach(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeMobileConnectivity_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogger<NativeMobileController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.NativeMobileConnectivity(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeMobileMemoryAndStorage_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogger<NativeMobileController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.NativeMobileMemoryAndStorage(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeMobileThirdParty_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogger<NativeMobileController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.NativeMobileThirdParty(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeMobileHardwareRequirements_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogger<NativeMobileController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.NativeMobileHardwareRequirements(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeMobileAdditionalInformation_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogger<NativeMobileController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.NativeMobileAdditionalInformation(id));
        }
    }
}
