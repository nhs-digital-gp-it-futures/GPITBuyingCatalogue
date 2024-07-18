using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.ScoringModels;

public static class InteroperabilityScoringModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Competition competition,
        Solution solution,
        List<Integration> integrations)
    {
        competition.CompetitionSolutions =
            new List<CompetitionSolution> { new(competition.Id, solution.CatalogueItemId) { Solution = solution } };
        competition.NonPriceElements = new()
        {
            IntegrationTypes = new List<IntegrationType>
            {
            },
        };

        var model = new InteroperabilityScoringModel(competition, integrations);

        model.CompetitionName.Should().Be(competition.Name);
        model.SolutionScores.Should().ContainSingle();
    }
}
