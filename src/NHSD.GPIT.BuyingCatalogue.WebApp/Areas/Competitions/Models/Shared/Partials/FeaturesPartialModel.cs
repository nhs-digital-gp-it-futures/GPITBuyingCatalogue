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
        bool hasReviewedCriteria,
        bool showAction = true)
    {
        InternalOrgId = internalOrgId;
        CompetitionId = competitionId;
        HasReviewedCriteria = hasReviewedCriteria;
        ShowAction = showAction;
        Requirements = featuresRequirements.ToList();
    }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public bool HasReviewedCriteria { get; set; }

    public bool ShowAction { get; set; }

    public string ReturnUrl { get; set; }

    public List<FeaturesCriteria> Requirements { get; set; }

    public bool CanDelete => ReturnUrl is null || Requirements?.Count > 1;
}
