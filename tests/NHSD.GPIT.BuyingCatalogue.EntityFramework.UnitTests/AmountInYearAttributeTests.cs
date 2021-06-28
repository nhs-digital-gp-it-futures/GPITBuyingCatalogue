using AutoFixture.Xunit2;
using FluentAssertions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests
{
    public static class AmountInYearAttributeTests
    {
        [Theory]
        [AutoData]
        public static void Constructor_Initializes_AmountInYear(
            [Frozen] int expectedAmountInYear,
            AmountInYearAttribute attribute)
        {
            attribute.AmountInYear.Should().Be(expectedAmountInYear);
        }
    }
}
