using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Ordering.Models
{

    public static class OrderItemRecipientTests
    {
        [Theory]
        [CommonInlineAutoData(null, 100)]
        [CommonInlineAutoData(TimeUnit.PerYear, 100)]
        [CommonInlineAutoData(TimeUnit.PerMonth, 1200)]
        public static void CalculateTotalCostPerYear_ReturnsExpectedValue(
            TimeUnit? unit,
            int expectedCost,
            OrderItemRecipient recipient)
        {
            recipient.Quantity = 10;

            var actualCost = recipient.CalculateTotalCostPerYear(10, unit);

            actualCost.Should().Be(expectedCost);
        }
    }
}
