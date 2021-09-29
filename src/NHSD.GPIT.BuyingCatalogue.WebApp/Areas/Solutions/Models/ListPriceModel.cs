using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class ListPriceModel : SolutionDisplayBaseModel
    {
        public ListPriceModel()
            : base()
        {
        }

        public ListPriceModel(CatalogueItem item)
            : base(item)
        {
            FlatListPrices = item.CataloguePrices.Select(cp => new PriceViewModel(cp)).ToList();
        }

        public override int Index => 3;

        public IList<PriceViewModel> FlatListPrices { get; set; }

        public bool HasFlatListPrices() => FlatListPrices?.Any() == true;
    }
}
