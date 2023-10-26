using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.ModelBinders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;

public class CompetitionWeightingsModel : NavBaseModel
{
    public CompetitionWeightingsModel()
    {
    }

    public CompetitionWeightingsModel(Competition competition)
    {
        CompetitionName = competition.Name;
        Price = competition.Weightings?.Price;
        NonPrice = competition.Weightings?.NonPrice;
        HasReviewedCriteria = competition.HasReviewedCriteria;
    }

    public string CompetitionName { get; set; }

    [Description("Price weighting")]
    [ModelBinder(typeof(NumberModelBinder))]
    public int? Price { get; set; }

    [Description("Non-price weighting")]
    [ModelBinder(typeof(NumberModelBinder))]
    public int? NonPrice { get; set; }

    public bool HasReviewedCriteria { get; set; }

    public string ContinueButton => HasReviewedCriteria ? "Continue" : "Save and continue";

    public override string Title => HasReviewedCriteria
        ? "Award criteria weightings"
        : "How would you like to weight your award criteria for this competition?";

    public override string Advice => HasReviewedCriteria
        ? "These are the award criteria weightings you gave for price and non-price elements."
        : "Give your chosen award criteria weightings based on how important they are to you.";
}
