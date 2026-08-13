using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Steps;

public class AwardCriteriaPage : BasePage
{
    public AwardCriteriaPage(IPage page) : base(page) { }

    public async Task SelectCriteriaAndContinueAsync(string criteria)
    {
        await AssertHeadingAsync("What criteria do you want to use to compare solutions?");
        await Page.GetByRole(AriaRole.Radio, new() { Name = criteria }).CheckAsync();
        await ClickSaveAndContinueAsync();
    }
}
