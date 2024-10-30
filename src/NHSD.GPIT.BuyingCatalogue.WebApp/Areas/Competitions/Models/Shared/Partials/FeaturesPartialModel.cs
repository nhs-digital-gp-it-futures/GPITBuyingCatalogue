using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.Shared.Partials;

public class FeaturesPartialModel
{
    [ExcludeFromCodeCoverage]
    public FeaturesPartialModel()
    {
    }

    public FeaturesPartialModel(
        string internalOrgId,
        int competitionId,
        ICollection<FeaturesCriteria> featuresRequirements,
        bool showChange = true,
        bool showDelete = true)
    {
        InternalOrgId = internalOrgId;
        CompetitionId = competitionId;

        ShowChange = showChange;
        ShowDelete = showDelete;
        Requirements = featuresRequirements.ToList();
    }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public bool ShowChange { get; set; }

    public bool ShowDelete { get; set; }

    public List<FeaturesCriteria> Requirements { get; set; }
}
