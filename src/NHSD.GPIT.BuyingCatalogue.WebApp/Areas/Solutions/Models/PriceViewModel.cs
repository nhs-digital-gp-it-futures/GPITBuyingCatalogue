using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class PriceViewModel
    {
        public PriceViewModel(CataloguePrice cp)
        {
            Price = cp.Price;
            Unit = cp.ToPriceUnitString();
        }

        public string CurrencyCode { get; init; } = "£";

        public decimal? Price { get; init; }

        public string Unit { get; init; }
    }
}
