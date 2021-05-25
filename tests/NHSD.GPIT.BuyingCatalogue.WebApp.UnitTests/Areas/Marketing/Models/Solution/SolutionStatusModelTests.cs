using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.Solution;
using NUnit.Framework;

// MJRTODO - Getting a namespace vs type clash when using Solution
namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.SolutionX
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class SolutionStatusModelTests
    {
        [TestCase("browser-based")]
        [TestCase("BROWSER-based")]
        public static void IsBrowserBased_ClientAppHasExpectedType_ReturnsTrue(string type)
        {
            var model = ModelFor(type);

            var actual = model.IsBrowserBased;

            actual.Should().BeTrue();
        }

        [TestCase("native-DESKTOP")]
        [TestCase("native-desktop")]
        public static void IsNativeDesktop_ClientAppHasExpectedType_ReturnsTrue(string type)
        {
            var model = ModelFor(type);

            var actual = model.IsNativeDesktop;

            actual.Should().BeTrue();
        }

        [TestCase("native-mobile")]
        [TestCase("native-MOBILE")]
        public static void IsNativeMobile_ClientAppHasExpectedType_ReturnsTrue(string type)
        {
            var model = ModelFor(type);

            var actual = model.IsNativeMobile;

            actual.Should().BeTrue();
        }
        
        [Test]
        public static void GetProperties_NoTypeInClientApp_ReturnFalse()
        {
            var model = ModelFor("some-type");

            model.IsBrowserBased.Should().BeFalse();
            model.IsNativeDesktop.Should().BeFalse();
            model.IsNativeMobile.Should().BeFalse();
        }
        
        [Test]
        public static void GetProperties_ClientAppIsNull_ReturnFalse()
        {
            var model = new SolutionStatusModel();
            model.ClientApplication.Should().BeNull();

            model.IsBrowserBased.Should().BeFalse();
            model.IsNativeDesktop.Should().BeFalse();
            model.IsNativeMobile.Should().BeFalse();
        }

        private static SolutionStatusModel ModelFor(string type) =>
            new()
            {
                ClientApplication =
                    new ClientApplication { ClientApplicationTypes = new HashSet<string> { type, } },
            };
    }
}
