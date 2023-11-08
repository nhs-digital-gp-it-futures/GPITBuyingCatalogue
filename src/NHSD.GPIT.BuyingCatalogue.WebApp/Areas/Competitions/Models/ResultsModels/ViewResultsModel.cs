using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;

public class ViewResultsModel : NavBaseModel
{
    public ViewResultsModel(
        Competition competition)
    {
        var selectedNonPriceElements = competition.NonPriceElements.GetNonPriceElements();

        InternalOrgId = competition.Organisation.InternalIdentifier;
        CompetitionId = competition.Id;
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

        WinningSolutions = competitionSolutionResults.Where(x => x.IsWinningSolution)
            .OrderBy(x => x.SolutionName)
            .ToList();
        OtherSolutionResults = competitionSolutionResults.Except(WinningSolutions).ToList();
    }

    public ViewResultsModel(
        Competition competition,
        FilterDetailsModel filterDetails,
        ICollection<CompetitionSolution> nonShortlistedSolutions)
        : this(competition)
    {
        NonShortlistedSolutions = nonShortlistedSolutions;

        FilterDetailsModel = new ReviewFilterModel(filterDetails);
    }

    public string InternalOrgId { get; set; }

    public int CompetitionId { get; set; }

    public string CompetitionName { get; set; }

    public bool IncludesNonPriceElements { get; set; }

    public string PdfUrl { get; set; }

    public bool HasMultipleWinners => WinningSolutions.Count > 1;

    public ReviewFilterModel FilterDetailsModel { get; set; }

    public Weightings AwardCriteriaWeightings { get; set; }

    public Dictionary<NonPriceElement, int> NonPriceElementWeightings { get; set; }

    public List<CompetitionSolutionResult> WinningSolutions { get; set; }

    public List<CompetitionSolutionResult> OtherSolutionResults { get; set; }

    public ICollection<CompetitionSolution> NonShortlistedSolutions { get; set; }
}
