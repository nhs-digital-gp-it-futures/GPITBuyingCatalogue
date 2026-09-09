using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Setup;

public class CompareAndScorePage : BasePage
{
    public CompareAndScorePage(IPage page) : base(page) { }

    public async Task AssertOnHubAsync() =>
        await AssertHeadingAsync("Compare and score shortlisted solutions");

    public async Task ScoreElementAsync(
        string element,
        string firstScore, string firstJustification,
        string secondScore, string secondJustification)
    {
        await Page.GetByRole(AriaRole.Row, new() { Name = element })
            .GetByRole(AriaRole.Link, new() { Name = "Start" })
            .ClickAsync();

        await AssertHeadingAsync($"Compare and score {element.ToLower()}");
        
        await EnterScoreAndJustificationAsync(0, firstScore, firstJustification);
        await EnterScoreAndJustificationAsync(1, secondScore, secondJustification);

        await ClickSaveAndContinueAsync();
        await AssertOnHubAsync();
    }

    private async Task EnterScoreAndJustificationAsync(int solutionIndex, string score, string justification)
    {
        await Page.Locator($"#SolutionScores_{solutionIndex}__Score").FillAsync(score);
        await Page.Locator($"#SolutionScores_{solutionIndex}__Justification").FillAsync(justification);
    }

    public async Task SaveAndContinueAsync() =>
        await ClickSaveAndContinueLinkAsync();
}
