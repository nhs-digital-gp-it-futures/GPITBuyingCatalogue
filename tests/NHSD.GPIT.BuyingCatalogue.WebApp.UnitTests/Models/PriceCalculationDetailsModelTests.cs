using AutoFixture.Xunit2;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Models
{
    public class PriceCalculationDetailsModelTests
    {
        [Theory]
        [AutoData]
        public void Constructor_SetsPartialName(
            CataloguePriceType priceType,
            CataloguePriceCalculationType calculationType)
        {
            var model = new PriceCalculationDetailsModel(
                CatalogueItemType.Solution,
                priceType,
                calculationType);

            Assert.Equal($"_{priceType.ToString()}{calculationType.ToString()}", model.PartialName);
        }

        [Theory]
        [InlineData(CataloguePriceType.Tiered, CataloguePriceCalculationType.Volume)]
        [InlineData(CataloguePriceType.Tiered, CataloguePriceCalculationType.Cumulative)]
        public void Constructor_WhenTieredAndNotSingleFixed_ReturnsCorrectTitle(CataloguePriceType priceType, CataloguePriceCalculationType calculationType)
        {
            var model = new PriceCalculationDetailsModel(
                CatalogueItemType.Solution,
                priceType,
                calculationType);

            Assert.Equal($"What is a {priceType.ToString().ToLowerInvariant()} {calculationType.Name().ToLowerInvariant()} price?", model.DetailsTitle);
        }

        [Theory]
        [InlineData(CataloguePriceType.Flat, CataloguePriceCalculationType.Cumulative)]
        [InlineData(CataloguePriceType.Flat, CataloguePriceCalculationType.Volume)]
        [InlineData(CataloguePriceType.Flat, CataloguePriceCalculationType.SingleFixed)]
        [InlineData(CataloguePriceType.Tiered, CataloguePriceCalculationType.SingleFixed)]
        public void Constructor_TieredAndFlat_ReturnsCorrectTitle(CataloguePriceType priceType, CataloguePriceCalculationType calculationType)
        {
            var model = new PriceCalculationDetailsModel(
                CatalogueItemType.Solution,
                priceType,
                calculationType);

            Assert.Equal($"What is a {priceType.ToString().ToLowerInvariant()} {calculationType.Name().ToLowerInvariant()} price?", model.DetailsTitle);
        }

        [Theory]
        [AutoData]
        public void Constructor_WhenCumulative_ReturnsCorrectHeading(
            CataloguePriceType priceType)
        {
            var model = new PriceCalculationDetailsModel(
                CatalogueItemType.Solution,
                priceType,
                CataloguePriceCalculationType.Cumulative);

            Assert.Equal($"The total price for this Catalogue Solution will be calculated cumulatively.", model.DetailsHeading);
        }

        [Theory]
        [InlineData(CataloguePriceType.Flat, CataloguePriceCalculationType.SingleFixed)]
        [InlineData(CataloguePriceType.Tiered, CataloguePriceCalculationType.SingleFixed)]
        public void Constructor_WhenNotCumulativeAndSingleFixed_ReturnsCorrectHeading(
            CataloguePriceType priceType,
            CataloguePriceCalculationType calculationType)
        {
            var model = new PriceCalculationDetailsModel(
                CatalogueItemType.Solution,
                priceType,
                calculationType);

            var expectedHeading = $"The total price for this Catalogue Solution will be calculated as a {priceType.ToString().ToLowerInvariant()}";
            expectedHeading += $" {calculationType.Name().ToLowerInvariant()} price (excluding VAT).";

            Assert.Equal(expectedHeading, model.DetailsHeading);
        }

        [Theory]
        [InlineData(CataloguePriceType.Flat, CataloguePriceCalculationType.Volume)]
        [InlineData(CataloguePriceType.Tiered, CataloguePriceCalculationType.Volume)]
        public void Constructor_WhenNotCumulativeAndNotSingleFixed_ReturnsCorrectHeading(
            CataloguePriceType priceType,
            CataloguePriceCalculationType calculationType)
        {
            var model = new PriceCalculationDetailsModel(
                CatalogueItemType.Solution,
                priceType,
                calculationType);

            var expectedHeading = $"The total price for this Catalogue Solution will be calculated as a {priceType.ToString().ToLowerInvariant()}";
            expectedHeading += " price based on volume (excluding VAT).";

            Assert.Equal(expectedHeading, model.DetailsHeading);
        }
    }
}
