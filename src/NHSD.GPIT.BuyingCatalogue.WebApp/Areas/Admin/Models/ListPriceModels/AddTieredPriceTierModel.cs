using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public class AddTieredPriceTierModel : NavBaseModel
    {
        public AddTieredPriceTierModel()
        {
        }

        public AddTieredPriceTierModel(CatalogueItem catalogueItem, CataloguePrice cataloguePrice)
        {
            CataloguePriceId = cataloguePrice.CataloguePriceId;
            CatalogueItemId = catalogueItem.Id;
            CatalogueItemName = catalogueItem.Name;
        }

        public static IList<SelectableRadioOption<bool>> AvailableRangeOptions => new List<SelectableRadioOption<bool>>
        {
            new("Infinite upper range", true),
            new("Set upper range", false),
        };

        public CatalogueItemId CatalogueItemId { get; set; }

        public string CatalogueItemName { get; set; }

        public int CataloguePriceId { get; set; }

        public decimal? Price { get; set; }

        public int? LowerRange { get; set; }

        public int? UpperRange { get; set; }

        public bool? IsInfiniteRange { get; set; }
    }
}
