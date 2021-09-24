using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class ListPriceModel : SolutionDisplayBaseModel
    {
        public ListPriceModel()
        {
        }

        public ListPriceModel(CatalogueItem catalogueItem)
        {
            FlatListPrices = 
                catalogueItem
                .CataloguePrices
                .Where(p => p.CataloguePriceType == CataloguePriceType.Flat)
                .Select(cp => new PriceViewModel(cp))
                .ToList();
        }

        public override int Index => 3;

        public IList<PriceViewModel> FlatListPrices { get; init; }

        public bool HasFlatListPrices() => FlatListPrices?.Any() == true;
    }
}
