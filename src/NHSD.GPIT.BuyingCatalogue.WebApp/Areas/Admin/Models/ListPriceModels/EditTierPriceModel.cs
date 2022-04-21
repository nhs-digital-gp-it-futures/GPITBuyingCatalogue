using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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
            Price = cataloguePriceTier.Price;
        }

        public string CatalogueItemName { get; set; }

        public decimal? Price { get; set; }

        public string Unit { get; set; }

        public int TierIndex { get; set; }
    }
}
