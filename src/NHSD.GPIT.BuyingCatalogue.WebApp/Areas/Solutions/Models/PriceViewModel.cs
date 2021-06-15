using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class PriceViewModel
    {
        public string CurrencyCode { get; set; }

        public decimal? Price { get; set; }

        public string Unit { get; set; }
    }
}
