using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Admin.Models
{
    public sealed class AddSolutionModel : NavBaseModel
    {
        public AddSolutionModel()
        {
            BackLink = "/admin/catalogue-solutions";
            BackLinkText = "Go back";
        }

        public int SupplierId { get; set; }

        public string SolutionName { get; set; }

        public string SupplierName { get; set; }

        public bool GpitFuturesFramework { get; set; }

        public bool FoundationSolutionFramework { get; set; }

        public bool DfocvcFramework { get; set; }

        public IDictionary<string, string> Suppliers { get; set; } = new Dictionary<string, string>();
    }
}
