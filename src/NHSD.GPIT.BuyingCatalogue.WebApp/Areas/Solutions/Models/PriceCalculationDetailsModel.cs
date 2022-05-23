using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Interfaces;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class PriceCalculationDetailsModel
    {
        public PriceCalculationDetailsModel(
            CatalogueItemType catalogueItemType,
            IPrice price)
        {
            var priceType = price.CataloguePriceType;
            var calculationType = price.CataloguePriceCalculationType;

            DetailsHeading = GetHeading(catalogueItemType, priceType, calculationType);
            DetailsTitle = GetTitle(priceType, calculationType);
            PartialName = $"_{priceType.ToString()}{calculationType.ToString()}";
        }

        public string DetailsHeading { get; }

        public string DetailsTitle { get; }

        public string PartialName { get; }

        private static string GetTitle(
            CataloguePriceType priceType,
            CataloguePriceCalculationType calculationType)
            => priceType == CataloguePriceType.Tiered && (calculationType != CataloguePriceCalculationType.SingleFixed)
                ? $"What is a {calculationType.Name().ToLowerInvariant()} price?"
                : $"What is a {priceType.ToString().ToLowerInvariant()} {calculationType.Name().ToLowerInvariant()} price?";

        private static string GetHeading(
            CatalogueItemType itemType,
            CataloguePriceType priceType,
            CataloguePriceCalculationType calculationType)
        {
            var baseHeading = $"The total price for this {itemType.Name()} will be calculated";
            if (calculationType == CataloguePriceCalculationType.Cumulative)
                return $"{baseHeading} cumulatively.";

            baseHeading += $" as a {priceType.ToString().ToLowerInvariant()}";

            return calculationType == CataloguePriceCalculationType.SingleFixed
                ? $"{baseHeading} {calculationType.Name().ToLowerInvariant()} price."
                : $"{baseHeading} price based on volume.";
        }
    }
}
