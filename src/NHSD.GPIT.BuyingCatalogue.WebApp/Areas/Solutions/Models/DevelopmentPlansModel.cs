using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class DevelopmentPlansModel : SolutionDisplayBaseModel, IDevelopmentPlans
    {
        public DevelopmentPlansModel(
            CatalogueItem catalogueItem,
            IList<WorkOffPlan> workoffPlans,
            CatalogueItemContentStatus contentStatus)
                : base(catalogueItem, contentStatus)
        {
            WorkOffPlans = workoffPlans;
            RoadmapLink = catalogueItem.Solution.RoadMap;
        }

        public DevelopmentPlansModel()
        {
        }

        public override int Index => 14;

        public string RoadmapLink { get; init; }

        public IList<WorkOffPlan> WorkOffPlans { get; init; }
    }
}
