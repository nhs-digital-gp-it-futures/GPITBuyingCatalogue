using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.HostingType;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class HostingTypeControllerTests
    {
        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(HostingTypeController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new HostingTypeController(null, Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new HostingTypeController(Mock.Of<ILogger<HostingTypeController>>(), null));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_HostingTypePublicCloud_InvalidId_ThrowsException(string id)
        {
            var controller = new HostingTypeController(Mock.Of<ILogger<HostingTypeController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.HostingTypePublicCloud(id));
        }

        [Test]
        public static void Post_HostingTypePublicCloud_NullModel_ThrowsException()
        {
            var controller = new HostingTypeController(Mock.Of<ILogger<HostingTypeController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.HostingTypePublicCloud((HostingTypePublicCloudModel)null));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_HostingTypePrivateCloud_InvalidId_ThrowsException(string id)
        {
            var controller = new HostingTypeController(Mock.Of<ILogger<HostingTypeController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.HostingTypePrivateCloud(id));
        }

        [Test]
        public static void Post_HostingTypePrivateCloud_NullModel_ThrowsException()
        {
            var controller = new HostingTypeController(Mock.Of<ILogger<HostingTypeController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.HostingTypePrivateCloud((HostingTypePrivateCloudModel)null));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_HostingTypeHybrid_InvalidId_ThrowsException(string id)
        {
            var controller = new HostingTypeController(Mock.Of<ILogger<HostingTypeController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.HostingTypeHybrid(id));
        }

        [Test]
        public static void Post_HostingTypeHybrid_NullModel_ThrowsException()
        {
            var controller = new HostingTypeController(Mock.Of<ILogger<HostingTypeController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.HostingTypeHybrid((HostingTypeHybridModel)null));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_HostingTypeOnPremise_InvalidId_ThrowsException(string id)
        {
            var controller = new HostingTypeController(Mock.Of<ILogger<HostingTypeController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.HostingTypeOnPremise(id));
        }

        [Test]
        public static void Post_HostingTypeOnPremise_NullModel_ThrowsException()
        {
            var controller = new HostingTypeController(Mock.Of<ILogger<HostingTypeController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.HostingTypeOnPremise((HostingTypeOnPremiseModel)null));
        }
    }
}
