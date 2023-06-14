using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SharedModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.SelectSolutionsModels;

public class SelectSolutionsModel : NavBaseModel
{
    public SelectSolutionsModel()
    {
    }

    public SelectSolutionsModel(
        string competitionName,
        IEnumerable<CompetitionSolution> competitionSolutions)
    {
        CompetitionName = competitionName;
        Solutions = competitionSolutions.Select(
                x => new SolutionModel(
                    x.Solution.CatalogueItem,
                    x.RequiredServices.Select(y => y.Service.CatalogueItem.Name).ToList(),
                    x.IsShortlisted))
            .OrderBy(x => x.SolutionName)
            .ToList();
    }

    public string CompetitionName { get; set; }

    public List<SolutionModel> Solutions { get; set; }

    public bool? IsDirectAward { get; set; }

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

    public string GetAdvice() => Solutions switch
    {
        [] or null => "There were no results from your chosen filter.",
        [not null] => "These are the results from your chosen filter.",
        [..] =>
            "These are the results from your chosen filter. You must provide a reason if any of the solutions listed are not taken through to your competition shortlist.",
    };
}
