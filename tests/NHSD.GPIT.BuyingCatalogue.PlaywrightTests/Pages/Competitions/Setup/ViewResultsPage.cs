using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Steps;

public class ViewResultsPage : BasePage
{
    private ILocator ConfirmCheckbox => Page.GetByLabel("I confirm I want to complete the competition and view the results");
    private ILocator ViewResultsButton => Page.GetByRole(AriaRole.Button, new() { Name = "View results" });

    public ViewResultsPage(IPage page) : base(page) { }

    public async Task ConfirmAndViewResultsAsync()
    {
        await AssertHeadingAsync("Are you ready to view the results for this competition?");
        await ConfirmCheckbox.CheckAsync();
        await ViewResultsButton.ClickAsync();
    }

    public async Task AssertResultsShownAsync() =>
        await AssertHeadingAsync("Competition results");
}
