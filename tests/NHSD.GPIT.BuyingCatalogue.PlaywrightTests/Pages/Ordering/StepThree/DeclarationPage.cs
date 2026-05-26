using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepThree;

public class DeclarationPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Declaration" });
    private ILocator AgreeCheckbox => Page.GetByRole(AriaRole.Checkbox, new() { Name = "I understand and agree to the" });

    public DeclarationPage(IPage page) : base(page) { }

    public async Task NavigateAndAgreeAsync()
    {
        await NavigationLink.ClickAsync();
        await AssertHeadingAsync("Declaration");
        await AgreeCheckbox.CheckAsync();
        await ClickSaveAndContinueAsync();
    }
}
