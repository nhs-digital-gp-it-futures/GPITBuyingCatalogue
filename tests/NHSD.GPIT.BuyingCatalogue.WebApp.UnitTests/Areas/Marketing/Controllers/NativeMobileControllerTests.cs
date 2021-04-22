﻿using System;
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
                _ = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(), null));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeMobileOperatingSystems_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.OperatingSystems(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeMobileMobileFirstApproach_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.MobileFirstApproach(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeMobileConnectivity_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.Connectivity(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeMobileMemoryAndStorage_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.MemoryAndStorage(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeMobileThirdParty_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.ThirdParty(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeMobileHardwareRequirements_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.HardwareRequirements(id));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_NativeMobileAdditionalInformation_InvalidId_ThrowsException(string id)
        {
            var controller = new NativeMobileController(Mock.Of<ILogWrapper<NativeMobileController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.AdditionalInformation(id));
        }
    }
}
