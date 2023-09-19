using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.ResultsModels;

public static class CompetitionSolutionResultTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Supplier supplier,
        Solution solution,
        Weightings weightings,
        Competition competition,
        SolutionScore solutionPriceScore,
        CompetitionSolution competitionSolution)
    {
        solution.CatalogueItem.Supplier = supplier;
        competition.Weightings = weightings;

        solutionPriceScore.ScoreType = ScoreType.Price;

        competitionSolution.Solution = solution;
        competitionSolution.Scores = new List<SolutionScore> { solutionPriceScore };

        var model = new CompetitionSolutionResult(competition, competitionSolution);

        model.SolutionName.Should().Be(solution.CatalogueItem.Name);
        model.SupplierName.Should().Be(supplier.LegalName);
        model.IncludesNonPriceElements.Should().Be(competition.IncludesNonPrice.GetValueOrDefault());
        model.Weightings.Should().Be(weightings);
        model.IsWinningSolution.Should().Be(competitionSolution.IsWinningSolution);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPriceScoreWeighting(
        Supplier supplier,
        Solution solution,
        Weightings weightings,
        Competition competition,
        SolutionScore solutionPriceScore,
        CompetitionSolution competitionSolution)
    {
        solution.CatalogueItem.Supplier = supplier;
        competition.Weightings = weightings;

        solutionPriceScore.ScoreType = ScoreType.Price;

        competitionSolution.Solution = solution;
        competitionSolution.Scores = new List<SolutionScore> { solutionPriceScore };

        var model = new CompetitionSolutionResult(competition, competitionSolution);

        model.PriceScoreWeighting.Should()
            .BeEquivalentTo(
                new CompetitionSolutionResult.PriceWeighting(
                    competitionSolution.CalculateTotalPrice(competition.ContractLength.GetValueOrDefault())
                        .GetValueOrDefault(),
                    solutionPriceScore.Score,
                    solutionPriceScore.WeightedScore));
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_SetsNonPriceElementWeights(
        Supplier supplier,
        Solution solution,
        Weightings weightings,
        Competition competition,
        SolutionScore solutionPriceScore,
        SolutionScore solutionImplementationScore,
        SolutionScore solutionInteroperabilityScore,
        SolutionScore solutionServiceLevelScore,
        CompetitionSolution competitionSolution)
    {
        var nonPriceElementScores = new List<SolutionScore>
        {
            solutionImplementationScore, solutionInteroperabilityScore, solutionServiceLevelScore,
        };

        solution.CatalogueItem.Supplier = supplier;

        competition.Weightings = weightings;
        competition.NonPriceElements = new()
        {
            Implementation = new(),
            Interoperability = new List<InteroperabilityCriteria> { new() },
            ServiceLevel = new(),
        };

        solutionImplementationScore.ScoreType = ScoreType.Implementation;
        solutionInteroperabilityScore.ScoreType = ScoreType.Interoperability;
        solutionServiceLevelScore.ScoreType = ScoreType.ServiceLevel;
        solutionPriceScore.ScoreType = ScoreType.Price;

        competitionSolution.Solution = solution;
        competitionSolution.Scores = new List<SolutionScore> { solutionPriceScore }.Concat(nonPriceElementScores).ToList();

        var model = new CompetitionSolutionResult(competition, competitionSolution);

        model.NonPriceElementWeights.Should()
            .BeEquivalentTo(
                nonPriceElementScores.Select(
                    x => new CompetitionSolutionResult.NonPriceElementWeighting(
                        x.ScoreType.AsNonPriceElement().GetValueOrDefault(),
                        x.Score,
                        x.WeightedScore)));
    }
}
