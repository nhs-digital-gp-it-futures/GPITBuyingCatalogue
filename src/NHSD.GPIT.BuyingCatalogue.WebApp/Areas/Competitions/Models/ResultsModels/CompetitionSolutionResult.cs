﻿using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;

public class CompetitionSolutionResult
{
    public CompetitionSolutionResult(
        Competition competition,
        CompetitionSolution competitionSolution)
    {
        SolutionName = competitionSolution.Solution.CatalogueItem.Name;
        SupplierName = competitionSolution.Solution.CatalogueItem.Supplier.LegalName;
        IncludesNonPriceElements = competition.IncludesNonPrice.GetValueOrDefault();
        Weightings = competition.Weightings;
        IsWinningSolution = competitionSolution.IsWinningSolution;

        var selectedNonPriceElements = competition.NonPriceElements.GetNonPriceElements();

        NonPriceElementWeights = selectedNonPriceElements.Select(
                x =>
                {
                    var nonPriceElementScore = competitionSolution.GetScoreByType(x.AsScoreType());

                    return new NonPriceElementWeighting(
                        x,
                        nonPriceElementScore.Score,
                        nonPriceElementScore.WeightedScore);
                })
            .OrderBy(x => x.NonPriceElement);

        var priceScore = competitionSolution.GetScoreByType(ScoreType.Price);

        PriceScoreWeighting = new PriceWeighting(
            competitionSolution.CalculateTotalPrice(competition.ContractLength.GetValueOrDefault()).GetValueOrDefault(),
            priceScore.Score,
            priceScore.WeightedScore);
    }

    public string SolutionName { get; set; }

    public string SupplierName { get; set; }

    public Weightings Weightings { get; set; }

    public bool IsWinningSolution { get; set; }

    public bool IncludesNonPriceElements { get; set; }

    public decimal TotalWeightedScore =>
        GetWeightedNonPriceElementWeight().GetValueOrDefault()
        + PriceScoreWeighting.WeightedScore;

    public IEnumerable<NonPriceElementWeighting> NonPriceElementWeights { get; set; }

    public PriceWeighting PriceScoreWeighting { get; set; }

    public decimal? GetTotalNonPriceElementWeight() => NonPriceElementWeights?.Sum(x => x.WeightedScore);

    public decimal? GetWeightedNonPriceElementWeight() =>
        CompetitionFormulas.CalculateWeightedScore(GetTotalNonPriceElementWeight().GetValueOrDefault(), Weightings?.NonPrice.GetValueOrDefault() ?? 0);

    public record NonPriceElementWeighting(NonPriceElement NonPriceElement, int Score, decimal WeightedScore);

    public record PriceWeighting(decimal Price, int Score, decimal WeightedScore);
}
