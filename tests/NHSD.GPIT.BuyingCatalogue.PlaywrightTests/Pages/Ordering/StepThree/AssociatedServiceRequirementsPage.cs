using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepThree;

public class AssociatedServiceRequirementsPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Associated service requirements" });

    public AssociatedServiceRequirementsPage(IPage page) : base(page) { }

    public async Task NavigateAndContinueAsync()
    {
        await NavigationLink.ClickAsync();
        await AssertHeadingAsync("Associated service requirements");
        await ClickSaveAndContinueAsync();
    }
}
