using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class ListPriceModel : SolutionDisplayBaseModel
    {
        public override int Index => 3;

        public IList<PriceViewModel> FlatListPrices { get; set; }

        public bool HasFlatListPrices() => FlatListPrices != null && FlatListPrices.Any();
    }
}
