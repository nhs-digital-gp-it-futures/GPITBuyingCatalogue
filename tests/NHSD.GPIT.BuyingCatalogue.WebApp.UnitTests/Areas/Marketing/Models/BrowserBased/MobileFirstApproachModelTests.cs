using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.BrowserBased
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class MobileFirstApproachModelTests
    {
        private static readonly string[] InvalidStrings = { null, string.Empty, "    " };
        
        [Test]
        public static void IsComplete_MobileFirstApproachHasValue_ReturnsTrue()
        {
            var model = new MobileFirstApproachModel { MobileFirstApproach = "someValue" };

            model.IsComplete.Should().BeTrue();
        }

        [TestCaseSource(nameof(InvalidStrings))]
        public static void IsComplete_MobileFirstApproachHasNoValue_ReturnsFalse(string invalid)
        {
            var model = new MobileFirstApproachModel { MobileFirstApproach = invalid };

            model.IsComplete.Should().BeFalse();
        }
    }
}
