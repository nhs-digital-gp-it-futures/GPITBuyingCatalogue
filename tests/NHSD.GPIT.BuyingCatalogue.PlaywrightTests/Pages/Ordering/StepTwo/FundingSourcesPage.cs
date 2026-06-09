using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepTwo;

/// <summary>
/// Page object for selecting funding sources during step 2 of the ordering journey.
/// Supports both single-source and multi-source funding flows.
/// </summary>
public class FundingSourcesPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Select funding sources" });

    public FundingSourcesPage(IPage page) : base(page) { }

    public async Task NavigateAsync() =>
        await NavigationLink.ClickAsync();

    public async Task SelectFundingAsync(string sourceFilter, string fundingType)
    {
        await AssertHeadingAsync("Funding sources");
        await Page.GetByRole(AriaRole.Row)
            .Filter(new() { HasText = sourceFilter })
            .GetByRole(AriaRole.Link, new() { Name = "Start" })
            .ClickAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = fundingType }).CheckAsync();
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Funding sources");
        await ClickSaveAndContinueAsync();
    }

    public async Task SelectFundingForSourcesAsync(string fundingType, params string[] sourceFilters)
    {
        foreach (var filter in sourceFilters)
        {
            await AssertHeadingAsync("Funding sources");
            await Page.GetByRole(AriaRole.Row)
                .Filter(new() { HasText = filter })
                .GetByRole(AriaRole.Link, new() { Name = "Start" })
                .ClickAsync();
            await Page.GetByRole(AriaRole.Radio, new() { Name = fundingType }).CheckAsync();
            await ClickSaveAndContinueAsync();
        }

        await AssertHeadingAsync("Funding sources");
        await ClickSaveAndContinueAsync();
    }
}
