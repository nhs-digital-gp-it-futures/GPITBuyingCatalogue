using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Steps;

public class CalculatePricePage : BasePage
{
    public CalculatePricePage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() =>
        await AssertHeadingAsync("Calculate price");

    public async Task StartFirstSolutionAsync() =>
        await Page.GetByRole(AriaRole.Link, new() { Name = "Start" }).First.ClickAsync();

    public async Task StartSolutionAsync(string solutionName) =>
        await Page.GetByRole(AriaRole.Row, new() { Name = solutionName })
            .GetByRole(AriaRole.Link, new() { Name = "Start", Exact = true })
            .ClickAsync();

    public async Task FinishAsync()
    {
        await AssertOnPageAsync();
        await ClickSaveAndContinueLinkAsync();
    }
}
