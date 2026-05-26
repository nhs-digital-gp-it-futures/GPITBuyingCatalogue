using System.Threading.Tasks;
using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepTwo;

public class FundingSourcesPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Select funding sources" });

    public FundingSourcesPage(IPage page) : base(page) { }

    public async Task NavigateAsync() =>
        await NavigationLink.ClickAsync();

    // Single funding source (used by Associated Service Only journeys)
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

    // Any number of funding sources — catalogue solution plus add-ons
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
