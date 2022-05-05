using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.SolutionSelection.Prices;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Order.Models.SolutionSelection.Prices
{
    public static class ConfirmPriceModelTests
    {
        [Theory]
        [CommonAutoData]
        public static void CatalogueItemIsNull_ThrowsException(int priceId)
        {
            FluentActions
                .Invoking(() => new ConfirmPriceModel(null, priceId))
                .Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [CommonAutoData]
        public static void WithValidArguments_PropertiesCorrectlySet(CatalogueItem item)
        {
            var price = item.CataloguePrices.First();

            var model = new ConfirmPriceModel(item, price.CataloguePriceId);

            model.Basis.Should().Be(price.ToPriceUnitString());
            model.ItemName.Should().Be(item.Name);
            model.ItemType.Should().Be(item.CatalogueItemType.Description());
            model.NumberOfTiers.Should().Be(price.CataloguePriceTiers.Count);

            foreach (var tier in model.Tiers)
            {
                var pricingTier = price.CataloguePriceTiers.Single(x => x.Id == tier.Id);

                tier.AgreedPrice.Should().Be($"{pricingTier.Price:#,##0.00##}");
                tier.Description.Should().Be(pricingTier.GetRangeDescription());
                tier.ListPrice.Should().Be(pricingTier.Price);
            }
        }
    }
}
