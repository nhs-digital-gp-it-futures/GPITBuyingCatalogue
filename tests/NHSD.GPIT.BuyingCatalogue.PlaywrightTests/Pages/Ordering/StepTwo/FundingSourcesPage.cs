using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepTwo;

public class FundingSourcesPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Select funding sources" });

    public FundingSourcesPage(IPage page) : base(page) { }

    public async Task NavigateAsync() =>
        await NavigationLink.ClickAsync();

    public async Task SelectFundingAsync(string solutionName, string fundingType)
    {
        await AssertHeadingAsync("Funding sources");
        await Page.GetByRole(AriaRole.Row)
            .Filter(new() { HasText = solutionName })
            .GetByRole(AriaRole.Link, new() { Name = "Start" })
            .ClickAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = fundingType }).CheckAsync();
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Funding sources");
        await ClickSaveAndContinueAsync();
    }
}
