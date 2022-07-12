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
            ICollection<CataloguePrice> prices)
        {
            TieredPrices = prices.Where(p => p.CataloguePriceType == CataloguePriceType.Tiered).ToList();
            FlatPrices = prices.Where(p => p.CataloguePriceType == CataloguePriceType.Flat).ToList();
        }

        public ManageListPricesModel(
            CatalogueItem catalogueItem,
            ICollection<CataloguePrice> prices)
        : this(prices)
        {
            CatalogueItemId = catalogueItem.Id;
            CatalogueItemName = catalogueItem.Name;
            CatalogueItemType = catalogueItem.CatalogueItemType;
        }

        public ManageListPricesModel(
            CatalogueItem solution,
            CatalogueItem service,
            ICollection<CataloguePrice> prices)
        : this(prices)
        {
            CatalogueItemId = solution.Id;
            ServiceId = service.Id;
            CatalogueItemName = service.Name;
            CatalogueItemType = service.CatalogueItemType;
        }

        public CatalogueItemId ServiceId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string CatalogueItemName { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public IList<CataloguePrice> TieredPrices { get; set; }

        public IList<CataloguePrice> FlatPrices { get; set; }

        public string AddListPriceUrl { get; set; }
    }
}
