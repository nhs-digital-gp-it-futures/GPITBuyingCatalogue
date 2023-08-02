using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionFeaturesModel : SolutionDisplayBaseModel
    {
        public SolutionFeaturesModel(CatalogueItem item, CatalogueItemContentStatus contentStatus)
            : base(item, contentStatus)
        {
            Features = item.Features();
        }

        public string[] Features { get; }

        public override int Index => 1;
    }
}
