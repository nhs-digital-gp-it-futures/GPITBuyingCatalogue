using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public class AddSolutionModel
    {
        public int SupplierId { get; set; }

        public string SolutionName { get; set; }

        public string SupplierName { get; set; }

        public bool GPITFuturesFramework { get; set; }

        public bool FoundationSolutionFramework { get; set; }

        public bool DFOCVCFramework { get; set; }

        public IDictionary<string, string> Suppliers { get; set; } = new Dictionary<string, string>();
    }
}
