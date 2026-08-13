using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Tests.Competitions;

public class CompetitionTests : BaseTest
{
    public CompetitionTests(TestServerFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [Fact]
    [Trait("Category", Categories.CompetitionJourney)]
    public async Task CompetitionPriceOnlyJourney()
    {
        await CompetitionPages.LoginAsync();
        await CompetitionPages.PrepareCompetitionAsync();
        await CompetitionPages.CompleteStepOneAsync();
        await CompetitionPages.DefineCompetitionCriteriaAsync();
        await CompetitionPages.CompareAndScoreSolutionsAsync();
        await CompetitionPages.FinishAndViewResultsAsync();
    }
}
