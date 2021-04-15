using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Controllers;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Solutions.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class VaccinationsControllerTests
    {
        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(VaccinationsController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Solutions");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new VaccinationsController(null));
        }

        [Test]
        public static void Get_Index_ReturnsDefaultView()
        {
            var controller = new VaccinationsController(Mock.Of<ILogger<VaccinationsController>>());

            var result = controller.Index();
            
            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
        }
    }
}
