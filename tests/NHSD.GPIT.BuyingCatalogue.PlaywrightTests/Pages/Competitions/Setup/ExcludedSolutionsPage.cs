using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Setup;

public class ExcludedSolutionsPage : BasePage
{
    private ILocator ReasonInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Why has this solution not" });

    public ExcludedSolutionsPage(IPage page) : base(page) { }

    public async Task EnterReasonAndContinueAsync(string reason)
    {
        await AssertHeadingAsync("Excluded solutions");
        await ReasonInput.FillAsync(reason);
        await ClickSaveAndContinueAsync();
    }
}
