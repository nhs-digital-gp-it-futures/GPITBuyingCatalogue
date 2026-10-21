using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.CommunityPharmacy.Models;

public class CommunityPharmacyDevelopmentPlansModel : CommunityPharmacySolutionDisplayBaseModel, IDevelopmentPlans
{
    public CommunityPharmacyDevelopmentPlansModel(CatalogueItem catalogueItem, IList<WorkOffPlan> workoffPlans, CatalogueItemContentStatus contentStatus)
        : base(catalogueItem, contentStatus)
    {
        WorkOffPlans = workoffPlans;
        RoadmapLink = catalogueItem.Solution.RoadMap;
    }

    public string RoadmapLink { get; init; }

    public IList<WorkOffPlan> WorkOffPlans { get; init; }

    public override int Index => 7;
}
