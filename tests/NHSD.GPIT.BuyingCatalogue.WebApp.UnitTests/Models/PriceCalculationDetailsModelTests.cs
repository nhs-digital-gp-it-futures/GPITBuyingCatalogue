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
        [AutoData]
        public void Constructor_WhenTieredAndSingleFixed_ReturnsCorrectTitle(
            CataloguePriceType priceType,
            CataloguePriceCalculationType calculationType)
        {
            var model = new PriceCalculationDetailsModel(
                CatalogueItemType.Solution,
                priceType,
                calculationType);

            if (priceType == CataloguePriceType.Tiered && calculationType != CataloguePriceCalculationType.SingleFixed)
                Assert.Equal($"What is a {calculationType.Name().ToLowerInvariant()} price?", model.DetailsTitle);
            else
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
        [AutoData]
        public void Constructor_WhenNotCumulative_ReturnsCorrectHeading(
            CataloguePriceType priceType,
            CataloguePriceCalculationType calculationType)
        {
            var model = new PriceCalculationDetailsModel(
                CatalogueItemType.Solution,
                priceType,
                calculationType);

            var expectedHeading = $"The total price for this Catalogue Solution will be calculated as a {priceType.ToString().ToLowerInvariant()}";
            if (calculationType == CataloguePriceCalculationType.SingleFixed)
                expectedHeading += $" {calculationType.Name().ToLowerInvariant()} price (excluding VAT).";
            else
                expectedHeading += " price based on volume (excluding VAT).";

            Assert.Equal(expectedHeading, model.DetailsHeading);
        }
    }
}
