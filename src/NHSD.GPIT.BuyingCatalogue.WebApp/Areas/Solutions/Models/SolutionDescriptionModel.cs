using System.ComponentModel.DataAnnotations;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class SolutionDescriptionModel : SolutionDisplayBaseModel
    {
        public SolutionDescriptionModel()
        {
        }

        public SolutionDescriptionModel(Solution solution)
            : base(solution)
        {
            Description = solution.FullDescription;
            Summary = solution.Summary;

            var item = solution.CatalogueItem;
            Frameworks = item.Frameworks().ToArray();
            IsFoundation = item.IsFoundation().ToYesNo();
            SupplierName = item.Supplier.Name;
        }

        public string Description { get; init; }

        [UIHint("TableListCell")]
        public string[] Frameworks { get; init; }

        public string IsFoundation { get; }

        public override int Index => 0;

        public string Summary { get; init; }

        public string SupplierName { get; }

        public string FrameworkTitle() => Frameworks is not null && Frameworks.Any() && Frameworks.Length > 1
            ? "Frameworks"
            : "Framework";

        public bool HasDescription() => !string.IsNullOrWhiteSpace(Description);

        public bool HasSummary() => !string.IsNullOrWhiteSpace(Summary);
    }
}
