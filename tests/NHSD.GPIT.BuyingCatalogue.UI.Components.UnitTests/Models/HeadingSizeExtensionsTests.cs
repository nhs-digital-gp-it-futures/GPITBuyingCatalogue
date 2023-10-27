using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.UnitTests.Models;

public static class HeadingSizeExtensionsTests
{
    [Theory]
    [InlineData(HeadingSize.ExtraSmall, "nhsuk-heading-xs")]
    [InlineData(HeadingSize.Small, "nhsuk-heading-s")]
    [InlineData(HeadingSize.Medium, "nhsuk-heading-m")]
    [InlineData(HeadingSize.Large, "nhsuk-heading-l")]
    [InlineData(HeadingSize.ExtraLarge, "nhsuk-heading-xl")]
    public static void ToHeading_ReturnsExpected(HeadingSize headingSize, string expected) =>
        headingSize.ToHeading().Should().Be(expected);
}
