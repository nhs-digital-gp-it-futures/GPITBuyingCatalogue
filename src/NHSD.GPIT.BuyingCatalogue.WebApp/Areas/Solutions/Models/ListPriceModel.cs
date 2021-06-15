using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class ListPriceModel : SolutionDisplayBaseModel
    {
        public override int Index => 3;

        public IEnumerable<PriceViewModel> FlatListPrices { get; set; }

        public IEnumerable<PriceViewModel> TierListPrices { get; set; }
    }
}
