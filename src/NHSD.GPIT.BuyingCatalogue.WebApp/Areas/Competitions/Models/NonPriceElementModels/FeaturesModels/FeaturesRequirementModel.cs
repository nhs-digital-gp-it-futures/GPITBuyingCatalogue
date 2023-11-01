using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.NonPriceElementModels.FeaturesModels;

public class FeaturesRequirementModel : NavBaseModel
{
    public FeaturesRequirementModel()
    {
    }

    public FeaturesRequirementModel(
        Competition competition)
    {
        CompetitionId = competition.Id;
        CompetitionName = competition.Name;
    }

    public FeaturesRequirementModel(
        Competition competition,
        FeaturesCriteria featureCriteria)
        : this(competition)
    {
        FeaturesCriteriaId = featureCriteria.Id;
        Requirements = featureCriteria.Requirements;
        SelectedCompliance = featureCriteria.Compliance;
    }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public int? FeaturesCriteriaId { get; set; }

    public string CompetitionName { get; set; }

    public string SelectedNonPriceElements { get; set; }

    public bool CanDelete => FeaturesCriteriaId is not null;

    public bool? IsAdding { get; set; }

    public string ReturnUrl { get; set; }

    [StringLength(1100)]
    public string Requirements { get; set; }

    public CompliancyLevel? SelectedCompliance { get; set; }

    public IEnumerable<SelectOption<CompliancyLevel>> AvailableComplianceLevels =>
        new List<SelectOption<CompliancyLevel>>
        {
            new(CompliancyLevel.Must.ToString(), CompliancyLevel.Must),
            new(CompliancyLevel.Should.ToString(), CompliancyLevel.Should),
        };
}
