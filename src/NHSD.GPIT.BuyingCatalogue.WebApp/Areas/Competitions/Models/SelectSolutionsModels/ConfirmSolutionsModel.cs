using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;

public class ConfirmSolutionsModel : NavBaseModel
{
    public ConfirmSolutionsModel()
    {
    }

    public ConfirmSolutionsModel(
        string competitionName,
        IList<CompetitionSolution> competitionSolutions)
    {
        CompetitionName = competitionName;

        ShortlistedSolutions = competitionSolutions.Where(x => x.IsShortlisted).ToList();
        NonShortlistedSolutions = competitionSolutions.Where(x => !x.IsShortlisted).ToList();
    }

    public string CompetitionName { get; set; }

    public IEnumerable<CompetitionSolution> ShortlistedSolutions { get; set; }

    public IEnumerable<CompetitionSolution> NonShortlistedSolutions { get; set; }
}
