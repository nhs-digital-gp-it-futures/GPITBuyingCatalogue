using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class PriceViewModel
    {
        public PriceViewModel(CataloguePrice cataloguePrice)
        {
            Price = cataloguePrice.Price;
            Unit = cataloguePrice.PricingUnitString();
        }

        public string CurrencyCode { get; } = "£";

        public decimal? Price { get; init; }

        public string Unit { get; init; }
    }
}
