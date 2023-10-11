using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels.FeaturesModels;

public class FeaturesRequirementsModel : NavBaseModel
{
    public FeaturesRequirementsModel()
    {
    }

    public FeaturesRequirementsModel(
        Competition competition)
    {
        CompetitionId = competition.Id;
        CompetitionName = competition.Name;
        Features = competition.NonPriceElements?.Features;
    }

    public int CompetitionId { get; set; }

    public string CompetitionName { get; set; }

    public string InternalOrgId { get; set; }

    public string SelectedNonPriceElements { get; set; }

    public ICollection<FeaturesCriteria> Features { get; set; }
}
