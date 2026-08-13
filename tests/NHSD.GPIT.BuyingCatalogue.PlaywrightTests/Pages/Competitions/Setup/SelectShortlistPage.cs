using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Setup;

public class SelectShortlistPage : BasePage
{
    private ILocator ShortlistDropdown => Page.GetByLabel("Which shortlist do you want");

    public SelectShortlistPage(IPage page) : base(page) { }

    public async Task SelectShortlistAndContinueAsync(string shortlistValue)
    {
        await AssertHeadingAsync("Select a shortlist");
        await ShortlistDropdown.SelectOptionAsync(shortlistValue);
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Results for this shortlist");
    }
}
