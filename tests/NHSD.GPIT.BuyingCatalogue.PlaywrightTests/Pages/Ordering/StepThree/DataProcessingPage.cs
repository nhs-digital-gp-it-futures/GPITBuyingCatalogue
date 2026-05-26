using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepThree;

public class DataProcessingPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Data processing information" });

    public DataProcessingPage(IPage page) : base(page) { }

    public async Task NavigateAndContinueAsync()
    {
        await NavigationLink.ClickAsync();
        await AssertHeadingAsync("Data processing information");
        await ClickSaveAndContinueAsync();
    }
}
