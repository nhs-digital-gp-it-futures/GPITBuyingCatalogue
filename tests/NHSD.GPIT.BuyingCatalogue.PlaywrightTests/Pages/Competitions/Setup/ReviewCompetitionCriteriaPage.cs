using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Steps;

public class ReviewCompetitionCriteriaPage : BasePage
{
    private ILocator ConfirmCheckbox => Page.GetByRole(AriaRole.Checkbox, new() { Name = "I confirm I want proceed with" });
    private ILocator ConfirmButton => Page.GetByRole(AriaRole.Button, new() { Name = "Confirm competition criteria" });

    public ReviewCompetitionCriteriaPage(IPage page) : base(page) { }

    public async Task ConfirmAsync()
    {
        await AssertHeadingAsync("Review competition criteria");
        await ConfirmCheckbox.CheckAsync();
        await ConfirmButton.ClickAsync();
    }
}
