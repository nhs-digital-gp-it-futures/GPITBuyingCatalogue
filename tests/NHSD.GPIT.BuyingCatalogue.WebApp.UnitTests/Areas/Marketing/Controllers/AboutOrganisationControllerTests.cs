using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.AboutOrganisation;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class AboutOrgainsationControllerTests
    {
        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(AboutOrganisationController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AboutOrganisationController(null, Mock.Of<ISolutionsService>()));
        }

        [Test]
        public static void Constructor_NullSolutionService_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new AboutOrganisationController(Mock.Of<ILogger<AboutOrganisationController>>(), null));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_AboutSupplier_InvalidId_ThrowsException(string id)
        {
            var controller = new AboutOrganisationController(Mock.Of<ILogger<AboutOrganisationController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.AboutSupplier(id));
        }

        [Test]
        public static void Post_AboutSupplier_NullModel_ThrowsException()
        {
            var controller = new AboutOrganisationController(Mock.Of<ILogger<AboutOrganisationController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.AboutSupplier((AboutSupplierModel)null));
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public static void Get_ContactDetails_InvalidId_ThrowsException(string id)
        {
            var controller = new AboutOrganisationController(Mock.Of<ILogger<AboutOrganisationController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentException>(() => controller.ContactDetails(id));
        }

        [Test]
        public static void Post_ContactDetails_NullModel_ThrowsException()
        {
            var controller = new AboutOrganisationController(Mock.Of<ILogger<AboutOrganisationController>>(), Mock.Of<ISolutionsService>());

            Assert.ThrowsAsync<ArgumentNullException>(() => controller.ContactDetails((ContactDetailsModel)null));
        }
    }
}
