using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models.ListPriceModels
{
    public class ListPriceTypeModel : NavBaseModel
    {
        public ListPriceTypeModel()
        {
        }

        public ListPriceTypeModel(CatalogueItem catalogueItem)
        {
            CatalogueItemName = catalogueItem.Name;
        }

        public string CatalogueItemName { get; set; }

        public CataloguePriceType? SelectedCataloguePriceType { get; set; }

        public IList<SelectableRadioOption<CataloguePriceType>> AvailableListPriceTypes => new List<SelectableRadioOption<CataloguePriceType>>
        {
            new(
                "Flat price",
                "This is where the price remains the same no matter what quantity is ordered.",
                CataloguePriceType.Flat),
            new(
                "Tiered price",
                "This is where the price per item changes based on the quantity ordered. Prices are different for each tier.",
                CataloguePriceType.Tiered),
        };
    }
}
