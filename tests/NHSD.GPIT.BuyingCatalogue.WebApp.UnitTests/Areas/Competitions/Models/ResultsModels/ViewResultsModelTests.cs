﻿using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ResultsModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.ResultsModels;

public static class ViewResultsModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        NonPriceWeights nonPriceWeights,
        Weightings weightings,
        Competition competition)
    {
        competition.NonPriceElements = new()
        {
            Implementation = new(),
            Interoperability = new List<InteroperabilityCriteria> { new() },
            ServiceLevel = new(),
            NonPriceWeights = nonPriceWeights,
        };

        competition.Weightings = weightings;

        var selectedNonPriceElements = competition.NonPriceElements.GetNonPriceElements();
        var expectedNonPriceElementWeightings = selectedNonPriceElements.ToDictionary(
            x => x,
            x => competition.NonPriceElements.GetNonPriceWeight(x).GetValueOrDefault());

        var model = new ViewResultsModel(competition);

        model.CompetitionName.Should().Be(competition.Name);
        model.AwardCriteriaWeightings.Should().Be(weightings);
        model.IncludesNonPriceElements.Should().Be(competition.IncludesNonPrice.GetValueOrDefault());
        model.NonPriceElementWeightings.Should().BeEquivalentTo(expectedNonPriceElementWeightings);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_SetsCompetitionSolutionResults(
        Weightings weightings,
        Competition competition,
        Supplier supplier,
        Solution otherSolution,
        Solution winningSolution,
        CompetitionSolution winningCompetitionSolution,
        CompetitionSolution otherCompetitionSolution)
    {
        otherSolution.CatalogueItem.Supplier = supplier;
        winningSolution.CatalogueItem.Supplier = supplier;

        winningCompetitionSolution.IsWinningSolution = true;
        winningCompetitionSolution.Solution = winningSolution;
        winningCompetitionSolution.Scores = new List<SolutionScore>
        {
            new(ScoreType.Price, 5, 2.5M),
            new(ScoreType.Implementation, 5, 2.5M),
            new(ScoreType.Interoperability, 5, 1.25M),
            new(ScoreType.ServiceLevel, 5, 1.25M),
        };

        otherCompetitionSolution.IsWinningSolution = false;
        otherCompetitionSolution.Solution = otherSolution;
        otherCompetitionSolution.Scores = new List<SolutionScore>
        {
            new(ScoreType.Price, 3, 1.5M),
            new(ScoreType.Implementation, 2, 1.25M),
            new(ScoreType.Interoperability, 2, 0.5M),
            new(ScoreType.ServiceLevel, 2, 0.5M),
        };

        competition.Weightings = weightings;
        competition.NonPriceElements = new()
        {
            Implementation = new(),
            Interoperability = new List<InteroperabilityCriteria> { new() },
            ServiceLevel = new(),
        };

        competition.CompetitionSolutions =
            new List<CompetitionSolution> { winningCompetitionSolution, otherCompetitionSolution };

        var model = new ViewResultsModel(competition);

        model.WinningSolutions.Should().ContainSingle();
        model.OtherSolutionResults.Should().ContainSingle();
    }
}
