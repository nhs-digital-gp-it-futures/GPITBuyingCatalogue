using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.BuyingCatalogue
{
    public static class CataloguePriceTests
    {
        [Theory]
        [CommonAutoData]
        public static void ToString_ReturnsCorrectString(CataloguePrice price)
        {
            var expected = $"£{price.Price.Value:F} {price.PricingUnit?.Description}".Trim();

            var actual = price.ToString();

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("test", null)]
        [InlineData(null, TimeUnit.PerMonth)]
        public static void ToPriceUnitString(string pricingUnitDescription, TimeUnit? timeUnit)
        {
            var expected = $"{pricingUnitDescription ?? string.Empty} {(timeUnit.HasValue ? timeUnit.Value.Description() : string.Empty)}".Trim();

            var cataloguePrice = new CataloguePrice
            {
                PricingUnit = new PricingUnit { Description = pricingUnitDescription },
                TimeUnit = timeUnit,
            };

            var actual = cataloguePrice.ToPriceUnitString();

            actual.Should().Be(expected);
        }
    }
}
