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
    }

    public string CompetitionName { get; set; }

    [Description("Price weighting")]
    [ModelBinder(typeof(NumberModelBinder))]
    public int? Price { get; set; }

    [Description("Non-price weighting")]
    [ModelBinder(typeof(NumberModelBinder))]
    public int? NonPrice { get; set; }
}
