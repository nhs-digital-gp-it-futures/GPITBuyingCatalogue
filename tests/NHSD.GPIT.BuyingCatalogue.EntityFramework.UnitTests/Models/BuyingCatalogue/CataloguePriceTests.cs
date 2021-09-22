using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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
    }
}
