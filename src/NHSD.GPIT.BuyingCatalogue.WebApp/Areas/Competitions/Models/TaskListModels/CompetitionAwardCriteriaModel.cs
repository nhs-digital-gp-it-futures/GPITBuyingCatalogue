using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;

public class CompetitionAwardCriteriaModel : NavBaseModel
{
    public CompetitionAwardCriteriaModel()
    {
    }

    public CompetitionAwardCriteriaModel(Competition competition)
    {
        CompetitionName = competition.Name;
        IncludesNonPrice = competition.IncludesNonPrice;
        HasReviewedCriteria = competition.HasReviewedCriteria;
    }

    public string CompetitionName { get; set; }

    public bool? IncludesNonPrice { get; set; }

    public bool HasReviewedCriteria { get; set; }

    public List<SelectOption<bool>> AwardCriteriaOptions { get; set; } = new()
    {
        new("Price only", false), new("Price and non-price elements", true),
    };

    public string ContinueButton => HasReviewedCriteria ? "Continue" : "Save and continue";

    public override string Title => HasReviewedCriteria
        ? "Award criteria"
        : "What criteria do you want to use to compare solutions?";

    public override string Advice => HasReviewedCriteria
        ? "This is the award criteria you selected to help you compare your shortlisted solutions."
        : "Select if you want to compare using price only or price and non-price elements such as features, implementation, interoperability or service levels.";
}
