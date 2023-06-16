using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SharedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;

public class CompetitionShortlistedSolutionsModel : NavBaseModel
{
    public CompetitionShortlistedSolutionsModel(
        Competition competition)
    {
        CompetitionName = competition.Name;
        Solutions = competition.CompetitionSolutions.Select(
                x => new SolutionModel(x))
            .OrderBy(x => x.SolutionName)
            .ToList();
    }

    public string CompetitionName { get; set; }

    public IEnumerable<SolutionModel> Solutions { get; set; }
}
