using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Setup;

public class BeforeYouStartPage : BasePage
{
    private ILocator CreateCompetitionButton => Page.GetByRole(AriaRole.Button, new() { Name = "Create competition" });

    public BeforeYouStartPage(IPage page) : base(page) { }

    public async Task ContinueAsync()
    {
        await AssertHeadingAsync("Before you create a competition");
        await CreateCompetitionButton.ClickAsync();
    }
}
