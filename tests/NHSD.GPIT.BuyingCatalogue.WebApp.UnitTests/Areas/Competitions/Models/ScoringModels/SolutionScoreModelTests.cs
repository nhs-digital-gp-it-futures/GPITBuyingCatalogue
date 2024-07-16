using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Models.ScoringModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.UnitTests.Areas.Competitions.Models.ScoringModels;

public static class SolutionScoreModelTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        Solution solution,
        int? score,
        string justification)
    {
        var model = new SolutionScoreModel(solution, score, justification);

        model.SolutionId.Should().Be(solution.CatalogueItemId);
        model.Solution.Should().Be(solution);
        model.Score.Should().Be(score);
        model.Justification.Should().Be(justification);
    }
}
