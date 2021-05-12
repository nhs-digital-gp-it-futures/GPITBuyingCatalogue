using System;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
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
        private static string[] InvalidStrings = { null, string.Empty, "    " };

        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(HostingTypeController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new HostingTypeController(null, Mock.Of<IMapper>(), Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(),Mock.Of<IMapper>(), null));
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_HostingTypePublicCloud_InvalidId_ThrowsException(string id)
        {
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(), Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.PublicCloud(id));
        }

        [Test]
        public static void Post_HostingTypePublicCloud_NullModel_ThrowsException()
        {
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(), Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.PublicCloud((PublicCloudModel)null));
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_HostingTypePrivateCloud_InvalidId_ThrowsException(string id)
        {
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(), Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.PrivateCloud(id));
        }

        [Test]
        public static void Post_HostingTypePrivateCloud_NullModel_ThrowsException()
        {
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(), Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.PrivateCloud((PrivateCloudModel)null));
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_HostingTypeHybrid_InvalidId_ThrowsException(string id)
        {
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(), Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.Hybrid(id));
        }

        [Test]
        public static void Post_HostingTypeHybrid_NullModel_ThrowsException()
        {
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(), Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.Hybrid((HybridModel)null));
        }

        [Test]
        [TestCaseSource(nameof(InvalidStrings))]
        public static void Get_HostingTypeOnPremise_InvalidId_ThrowsException(string id)
        {
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(), Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.OnPremise(id));
        }

        [Test]
        public static void Post_HostingTypeOnPremise_NullModel_ThrowsException()
        {
            var controller = new HostingTypeController(Mock.Of<ILogWrapper<HostingTypeController>>(), Mock.Of<IMapper>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.OnPremise((OnPremiseModel)null));
        }
    }
}
