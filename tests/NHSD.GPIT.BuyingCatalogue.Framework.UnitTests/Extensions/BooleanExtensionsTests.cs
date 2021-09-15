using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Extensions
{
    public static class BooleanExtensionsTests
    {
        [Theory]
        [InlineData(null, "")]
        [InlineData(true, "Yes")]
        [InlineData(false, "No")]
        public static void BooleanExtension_ToYesNo_ReturnsYesOrNoCorrectly(bool? value, string expected)
        {
            var result = value.ToYesNo();

            result.Should().Be(expected);
        }
    }
}
