using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public class ManageListPricesModel : NavBaseModel
    {
        public ManageListPricesModel(
            CatalogueItem catalogueItem,
            ICollection<CataloguePrice> prices)
        {
            CatalogueItemId = catalogueItem.Id;
            CatalogueItemName = catalogueItem.Name;
            TieredPrices = prices.Where(p => p.CataloguePriceType == CataloguePriceType.Tiered).ToList();
            FlatPrices = prices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();
        }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string CatalogueItemName { get; set; }

        public IList<CataloguePrice> TieredPrices { get; set; }

        public IList<CataloguePrice> FlatPrices { get; set; }
    }
}
