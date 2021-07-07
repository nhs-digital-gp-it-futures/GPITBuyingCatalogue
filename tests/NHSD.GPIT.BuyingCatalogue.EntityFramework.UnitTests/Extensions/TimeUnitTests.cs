using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Extensions
{
    public static class TimeUnitTests
    {
        [Theory]
        [InlineData(TimeUnit.PerMonth, 12)]
        [InlineData(TimeUnit.PerYear, 1)]
        public static void EachTimeUnit_HasExpectedAmountInYear(TimeUnit timeUnit, int expectedAmountInYear)
        {
            var actualAmountInYear = timeUnit.AmountInYear();

            actualAmountInYear.Should().Be(expectedAmountInYear);
        }

        [Theory]
        [InlineData(TimeUnit.PerMonth, "per month")]
        [InlineData(TimeUnit.PerYear, "per year")]
        public static void EachTimeUnit_HasExpectedDescription(TimeUnit timeUnit, string expectedDescription)
        {
            var actualDescription = timeUnit.Description();

            actualDescription.Should().Be(expectedDescription);
        }

        [Theory]
        [InlineData(TimeUnit.PerMonth, "month")]
        [InlineData(TimeUnit.PerYear, "year")]
        public static void EachTimeUnit_HasExpectedDisplayName(TimeUnit timeUnit, string expectedDisplayName)
        {
            var actualDisplayName = timeUnit.Name();

            actualDisplayName.Should().Be(expectedDisplayName);
        }

        [Theory]
        [InlineData(TimeUnit.PerMonth, "PerMonth")]
        [InlineData(TimeUnit.PerYear, "PerYear")]
        public static void EachTimeUnit_HasExpectedEnumMemberName(TimeUnit timeUnit, string expectedEnumMemberName)
        {
            var actualDisplayName = timeUnit.EnumMemberName();

            actualDisplayName.Should().Be(expectedEnumMemberName);
        }
    }
}
