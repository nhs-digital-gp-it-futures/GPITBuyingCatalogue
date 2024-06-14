using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.Competitions;

public static class SolutionScoreTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        ScoreType scoreType,
        int score)
    {
        var solutionScore = new SolutionScore(scoreType, score);

        solutionScore.ScoreType.Should().Be(scoreType);
        solutionScore.Score.Should().Be(score);
        solutionScore.SolutionId.Should().Be(default(CatalogueItemId));
        solutionScore.CompetitionId.Should().Be(default);
        solutionScore.Id.Should().Be(default);
    }
}
