using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionFeaturesModel : SolutionDisplayBaseModel
    {
        public SolutionFeaturesModel(CatalogueItem item, CatalogueItemContentStatus contentStatus)
            : base(item, contentStatus)
        {
            Features = item.Features();
            SupplierName = item.Supplier.Name;
            IsFoundation = item.Solution.FrameworkSolutions.Any(fs => fs.IsFoundation).ToYesNo();
        }

        public string[] Features { get; }

        public string SupplierName { get; }

        public string IsFoundation { get; }

        public string FrameworkTitle() => Frameworks is not null && Frameworks.Any() && Frameworks.Count > 1
            ? "Frameworks"
            : "Framework";

        public override int Index => 1;
    }
}
