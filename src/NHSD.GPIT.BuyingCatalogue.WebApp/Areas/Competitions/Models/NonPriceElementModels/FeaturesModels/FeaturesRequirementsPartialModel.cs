using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels.FeaturesModels;

public class FeaturesRequirementsPartialModel
{
    [ExcludeFromCodeCoverage]
    public FeaturesRequirementsPartialModel()
    {
    }

    public FeaturesRequirementsPartialModel(
        string internalOrgId,
        int competitionId,
        ICollection<FeaturesCriteria> featuresRequirements)
    {
        InternalOrgId = internalOrgId;
        CompetitionId = competitionId;

        MustRequirements = featuresRequirements.Where(x => x.Compliance == CompliancyLevel.Must).ToList();
        ShouldRequirements = featuresRequirements.Where(x => x.Compliance == CompliancyLevel.Should).ToList();
    }

    public FeaturesRequirementsPartialModel(
        string internalOrgId,
        int competitionId,
        ICollection<FeaturesCriteria> featuresRequirements,
        bool hasReviewedCriteria)
        : this(internalOrgId, competitionId, featuresRequirements)
    {
        HasReviewedCriteria = hasReviewedCriteria;
    }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public string ReturnUrl { get; set; }

    public string SelectedNonPriceElements { get; set; }

    public bool HasReviewedCriteria { get; set; }

    public List<FeaturesCriteria> MustRequirements { get; set; }

    public List<FeaturesCriteria> ShouldRequirements { get; set; }
}
