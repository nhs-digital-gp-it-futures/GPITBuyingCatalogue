using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionDescriptionModel : SolutionDisplayBaseModel
    {
        public SolutionDescriptionModel()
        {
        }

        public SolutionDescriptionModel(CatalogueItem solution)
        {
            SolutionId = solution.Id;
            SolutionName = solution.Name;

            Description = solution.Solution?.FullDescription;
            Frameworks = solution.Frameworks().ToArray();
            IsFoundation = solution.IsFoundation().ToYesNo();
            Summary = solution.Solution?.Summary;
            SupplierName = solution.Supplier.Name;
        }

        public string Description { get; set; }

        [UIHint("TableListCell")]
        public string[] Frameworks { get; set; }

        public string IsFoundation { get; set; }

        public override int Index => 0;

        public string Summary { get; set; }

        public string SupplierName { get; set; }

        public string FrameworkTitle() => Frameworks is not null && Frameworks.Any() && Frameworks.Length > 1
            ? "Frameworks"
            : "Framework";

        public bool HasDescription() => !string.IsNullOrWhiteSpace(Description);

        public bool HasSummary() => !string.IsNullOrWhiteSpace(Summary);
    }
}
