using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.ScoringModels;

public static class ImplementationScoringModelTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Competition competition,
        Solution solution,
        string requirements)
    {
        competition.CompetitionSolutions =
            new List<CompetitionSolution> { new(competition.Id, solution.CatalogueItemId) { Solution = solution } };
        competition.NonPriceElements = new() { Implementation = new() { Requirements = requirements } };

        var model = new ImplementationScoringModel(competition);

        model.CompetitionName.Should().Be(competition.Name);
        model.Implementation.Should().Be(competition.NonPriceElements.Implementation.Requirements);
        model.SolutionScores.Should().ContainSingle();
    }
}
