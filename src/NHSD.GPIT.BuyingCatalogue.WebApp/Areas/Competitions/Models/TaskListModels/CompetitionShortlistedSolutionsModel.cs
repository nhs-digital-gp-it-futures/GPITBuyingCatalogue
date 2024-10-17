using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.TaskListModels;

public class CompetitionShortlistedSolutionsModel : NavBaseModel
{
    public CompetitionShortlistedSolutionsModel(
        Competition competition, string frameworkName)
    {
        CompetitionName = competition.Name;
        Solutions = competition.CompetitionSolutions.Select(
                x => new SolutionModel(x))
            .OrderBy(x => x.SolutionName)
            .ToList();
        FrameworkName = frameworkName;
    }

    public string CompetitionName { get; set; }

    public string FrameworkName { get; set; }

    public IEnumerable<SolutionModel> Solutions { get; set; }
}
