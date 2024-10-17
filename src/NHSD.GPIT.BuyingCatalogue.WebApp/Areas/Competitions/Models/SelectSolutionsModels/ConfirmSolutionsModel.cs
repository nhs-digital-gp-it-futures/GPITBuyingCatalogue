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
        CompetitionSolutions = competitionSolutions;
    }

    public int CompetitionId { get; set; }

    public string InternalOrgId { get; set; }

    public string CompetitionName { get; set; }

    public IEnumerable<CompetitionSolution> CompetitionSolutions { get; set; }

    public IEnumerable<CompetitionSolution> ShortlistedSolutions => CompetitionSolutions?.Where(x => x.IsShortlisted).ToList();

    public IEnumerable<CompetitionSolution> NonShortlistedSolutions => CompetitionSolutions?.Where(x => !x.IsShortlisted).ToList();

    public bool ConfirmShortlist { get; set; }
}
