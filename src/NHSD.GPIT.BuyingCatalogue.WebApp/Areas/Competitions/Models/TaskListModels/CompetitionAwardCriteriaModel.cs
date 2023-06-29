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
    }

    public string CompetitionName { get; set; }

    public bool? IncludesNonPrice { get; set; }

    public List<SelectOption<bool>> AwardCriteriaOptions { get; set; } = new()
    {
        new("Price only", false), new("Price and non-price elements", true),
    };
}
