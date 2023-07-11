using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionFeaturesModel : SolutionDisplayBaseModel
    {
        public SolutionFeaturesModel(CatalogueItem item, CatalogueItemContentStatus contentStatus, SolutionDescriptionModel solutionDescription)
            : base(item, contentStatus)
        {
            Features = item.Features();
            SolutionDescription = solutionDescription;
        }

        public string[] Features { get; }

        public override int Index => 1;

        public SolutionDescriptionModel SolutionDescription { get; set; }
    }
}
