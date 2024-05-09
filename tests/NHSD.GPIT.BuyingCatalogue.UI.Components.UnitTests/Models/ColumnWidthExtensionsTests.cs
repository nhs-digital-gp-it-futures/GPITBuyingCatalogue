using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.UI.Components.UnitTests.Models;

public static class ColumnWidthExtensionsTests
{
    [Theory]
    [InlineData(ColumnWidth.Half, "nhsuk-grid-column-one-half")]
    [InlineData(ColumnWidth.OneThird, "nhsuk-grid-column-one-third")]
    public static void ToClass_ReturnsExpected(ColumnWidth columnWidth, string expected)
        => columnWidth.ToClass().Should().Be(expected);

    [Fact]
    public static void ToClass_InvalidSelection_ThrowsArgumentOutOfRangeException() => FluentActions
        .Invoking(() => ((ColumnWidth)int.MaxValue).ToClass())
        .Should()
        .Throw<ArgumentOutOfRangeException>();
}
