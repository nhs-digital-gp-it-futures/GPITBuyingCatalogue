using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Extensions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public class EditTierPriceModel : NavBaseModel
    {
        public EditTierPriceModel()
        {
        }

        public EditTierPriceModel(
            CatalogueItem catalogueItem,
            CataloguePrice cataloguePrice,
            CataloguePriceTier cataloguePriceTier)
        {
            CatalogueItemName = catalogueItem.Name;
            Unit = cataloguePrice.PricingUnit.Description;
            InputPrice = $"{cataloguePriceTier.Price}";
        }

        public string CatalogueItemName { get; set; }

        public decimal? Price => InputPrice.AsNullableDecimal();

        public string InputPrice { get; set; }

        public string Unit { get; set; }

        public int TierIndex { get; set; }
    }
}
