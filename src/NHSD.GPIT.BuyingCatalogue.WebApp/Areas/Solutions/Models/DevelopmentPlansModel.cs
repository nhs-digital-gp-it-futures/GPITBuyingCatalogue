using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public sealed class DevelopmentPlansModel : SolutionDisplayBaseModel
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

        public override int Index => 13;

        public string RoadmapLink { get; init; }

        public IList<WorkOffPlan> WorkOffPlans { get; init; }
    }
}
