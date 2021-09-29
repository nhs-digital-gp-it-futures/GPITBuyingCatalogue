using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionFeaturesModel : SolutionDisplayBaseModel
    {
        public SolutionFeaturesModel(CatalogueItem item)
            : base(item)
        {
            Features = item.Features();
        }

        public string[] Features { get; }

        public override int Index => 1;
    }
}
