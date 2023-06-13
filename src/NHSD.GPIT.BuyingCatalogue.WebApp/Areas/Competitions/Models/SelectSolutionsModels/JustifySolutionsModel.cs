using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;

public class JustifySolutionsModel : NavBaseModel
{
    public JustifySolutionsModel()
    {
    }

    public JustifySolutionsModel(string competitionName, IEnumerable<CompetitionSolution> nonShortlistedSolutions)
    {
        CompetitionName = competitionName;
        Solutions = nonShortlistedSolutions.Select(
                x => new SolutionJustificationModel(x.Solution.CatalogueItem, x.Justification))
            .OrderBy(x => x.SolutionName)
            .ToList();
    }

    public string CompetitionName { get; set; }

    public List<SolutionJustificationModel> Solutions { get; set; }
}
