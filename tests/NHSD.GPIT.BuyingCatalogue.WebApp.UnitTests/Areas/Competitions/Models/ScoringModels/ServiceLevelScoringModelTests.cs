using System.Collections.Generic;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.ScoringModels;

public static class ServiceLevelScoringModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Competition competition,
        Solution solution,
        ServiceLevelCriteria serviceLevelCriteria)
    {
        competition.CompetitionSolutions =
            new List<CompetitionSolution> { new(competition.Id, solution.CatalogueItemId) { Solution = solution } };
        competition.NonPriceElements = new() { ServiceLevel = serviceLevelCriteria };

        var model = new ServiceLevelScoringModel(competition);

        model.CompetitionName.Should().Be(competition.Name);
        model.From.Should().Be(serviceLevelCriteria.TimeFrom);
        model.Until.Should().Be(serviceLevelCriteria.TimeUntil);
        model.ApplicableDays.Should().BeEquivalentTo(serviceLevelCriteria.ApplicableDays);
        model.SolutionScores.Should().ContainSingle();
    }
}
