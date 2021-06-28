using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.TestData;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Marketing.Models.BrowserBased;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Marketing.Models.BrowserBased
{
    public static class MobileFirstApproachModelTests
    {
        [Fact]
        public static void IsComplete_MobileFirstApproachHasValue_ReturnsTrue()
        {
            var model = new MobileFirstApproachModel { MobileFirstApproach = "someValue" };

            model.IsComplete.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(InvalidStringData.TestData), MemberType = typeof(InvalidStringData))]
        public static void IsComplete_MobileFirstApproachHasNoValue_ReturnsFalse(string invalid)
        {
            var model = new MobileFirstApproachModel { MobileFirstApproach = invalid };

            model.IsComplete.Should().BeFalse();
        }
    }
}
