using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class OrganisationControllerTests
    {
        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new OrganisationController(null));
        }

        [Test]
        public static void Get_NewOrder_ReturnsViewResult()
        {
            var controller = new OrganisationController(Mock.Of<ILogger<OrganisationController>>());

            var result = controller.NewOrder();
            
            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
        }
    }
}
