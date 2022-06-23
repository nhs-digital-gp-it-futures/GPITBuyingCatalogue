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
                "A flat price means a buyer is presented with a single pricing unit.",
                CataloguePriceType.Flat),
            new(
                "Tiered price",
                "A tiered price means a buyer is presented with multiple pricing units.",
                CataloguePriceType.Tiered),
        };
    }
}
