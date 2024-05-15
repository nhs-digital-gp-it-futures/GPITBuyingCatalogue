using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;

public class SelectSolutionsModel : NavBaseModel
{
    public SelectSolutionsModel()
    {
    }

    public SelectSolutionsModel(
        string competitionName,
        IEnumerable<CompetitionSolution> competitionSolutions,
        string frameworkName,
        FilterDetailsModel filterDetails)
    {
        CompetitionName = competitionName;
        Solutions = competitionSolutions.Select(
                x => new SolutionModel(x))
            .OrderBy(x => x.SolutionName)
            .ToList();
        FrameworkName = frameworkName;
        ReviewFilter = new ReviewFilterModel(filterDetails) { InExpander = true };
    }

    public string CompetitionName { get; set; }

    public List<SolutionModel> Solutions { get; set; }

    public string FrameworkName { get; set; }

    public bool? IsDirectAward { get; set; }

    public ReviewFilterModel ReviewFilter { get; set; }

    public List<SelectOption<bool>> DirectAwardOptions => new()
    {
        new(
            "Yes, I want to use a direct award ",
            "You’ll be able to place an order for this solution without carrying out a competition.",
            true),
        new(
            "No, I want to abandon this competition",
            "Abandoning this competition will delete it permanently.",
            false),
    };

    public bool HasNoSolutions() => !Solutions.Any();

    public bool HasSingleSolution() => Solutions.Count == 1;
}
