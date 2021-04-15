using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Controllers;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Controllers
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SolutionControllerTests
    {
        [Test]
        public static void ClassIsCorrectlyDecorated()
        {
            typeof(SolutionController).Should().BeDecoratedWith<AreaAttribute>(x => x.RouteValue == "Marketing");
        }

        [Test]
        public static void Constructor_NullLogging_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _ = new SolutionController(null));
        }

        [Test]
        public static void Get_Index_ReturnsViewResult()
        {
            var controller = new SolutionController(Mock.Of<ILogger<SolutionController>>());

            var result = controller.Index("123");
            
            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
        }

        [Test]
        public static void Get_Preview_ReturnsDefaultView()
        {
            var controller = new SolutionController(Mock.Of<ILogger<SolutionController>>());

            var result = controller.Preview("123");

            Assert.That(result, Is.InstanceOf(typeof(ViewResult)));
            Assert.IsNull(((ViewResult)result).ViewName);
        }
    }
}
