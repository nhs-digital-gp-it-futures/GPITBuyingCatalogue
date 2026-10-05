using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Setup;

public class ConfirmShortlistPage : BasePage
{
    private ILocator ContinueWithShortlistOption => Page.GetByText("I want to continue with this");
    private ILocator ConfirmShortlistButton => Page.GetByRole(AriaRole.Button, new() { Name = "Confirm shortlist" });

    public ConfirmShortlistPage(IPage page) : base(page) { }

    public async Task ConfirmAsync()
    {
        await AssertHeadingAsync("Confirm shortlisted solutions");
        await ContinueWithShortlistOption.ClickAsync();
        await ConfirmShortlistButton.ClickAsync();
    }
}
