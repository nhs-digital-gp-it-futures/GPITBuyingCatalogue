using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionDescriptionModel : SolutionDisplayBaseModel
    {
        public string Description { get; set; }

        [UIHint("TableListCell")]
        public string[] Frameworks { get; set; }

        public string IsFoundation { get; set; }

        public override int Index => 0;

        public string Summary { get; set; }

        public string SupplierName { get; set; }

        public string FrameworkTitle() => Frameworks != null && Frameworks.Any() && Frameworks.Length > 1
            ? "Frameworks"
            : "Framework";

        public bool HasDescription() => !string.IsNullOrWhiteSpace(Description);

        public bool HasSummary() => !string.IsNullOrWhiteSpace(Summary);
    }
}
