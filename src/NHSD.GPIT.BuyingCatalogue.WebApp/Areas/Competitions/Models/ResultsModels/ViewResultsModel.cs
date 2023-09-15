using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;

public class ViewResultsModel : NavBaseModel
{
    public ViewResultsModel(
        Competition competition)
    {
        var selectedNonPriceElements = competition.NonPriceElements.GetNonPriceElements();

        CompetitionName = competition.Name;
        IncludesNonPriceElements = competition.IncludesNonPrice.GetValueOrDefault();
        AwardCriteriaWeightings = competition.Weightings;

        NonPriceElementWeightings = selectedNonPriceElements.ToDictionary(
            x => x,
            x => competition.NonPriceElements.GetNonPriceWeight(x).GetValueOrDefault());

        var competitionSolutionResults = competition.CompetitionSolutions
            .Select(x => new CompetitionSolutionResult(competition, x))
            .OrderByDescending(x => x.TotalWeightedScore)
            .ToList();

        WinningSolutions = competitionSolutionResults.Where(x => x.IsWinningSolution).OrderBy(x => x.SolutionName).ToList();
        OtherSolutionResults = competitionSolutionResults.Except(WinningSolutions).ToList();
    }

    public string CompetitionName { get; set; }

    public bool IncludesNonPriceElements { get; set; }

    public string PdfUrl { get; set; }

    public Weightings AwardCriteriaWeightings { get; set; }

    public Dictionary<NonPriceElement, int> NonPriceElementWeightings { get; set; }

    public List<CompetitionSolutionResult> WinningSolutions { get; set; }

    public List<CompetitionSolutionResult> OtherSolutionResults { get; set; }
}
