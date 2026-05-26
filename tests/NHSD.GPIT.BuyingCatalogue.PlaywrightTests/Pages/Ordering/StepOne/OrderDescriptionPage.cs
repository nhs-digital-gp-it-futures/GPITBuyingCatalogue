using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepOne;

public class OrderDescriptionPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Order description" });
    private ILocator DescriptionInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Order description" });

    public OrderDescriptionPage(IPage page) : base(page) { }

    public async Task NavigateAsync() =>
        await NavigationLink.ClickAsync();

    public async Task EnterDescriptionAsync(string description)
    {
        await DescriptionInput.ClearAsync();
        await DescriptionInput.FillAsync(description);
        await ClickSaveAndContinueAsync();
    }
}
