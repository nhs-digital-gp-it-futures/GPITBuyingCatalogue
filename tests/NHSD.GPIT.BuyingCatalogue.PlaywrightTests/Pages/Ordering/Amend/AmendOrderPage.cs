using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.Amend;

public class AmendOrderPage : BasePage
{
    private ILocator AmendContractLink => Page.GetByRole(AriaRole.Link, new() { Name = "Amend contract" });
    private ILocator AmendOrderButton => Page.GetByRole(AriaRole.Button, new() { Name = "Amend order" });

    public AmendOrderPage(IPage page) : base(page) { }

    public async Task OpenCompletedOrderAsync(int orderId)
    {
        await Page.Locator($"[data-test-id=\"link-C0{orderId}-01\"]").ClickAsync();
        await AssertHeadingAsync("Order confirmed");
    }

    public async Task StartAmendAsync()
    {
        await AmendContractLink.ClickAsync();
        await AssertHeadingAsync("Amending an order");
        await AmendOrderButton.ClickAsync();
    }
}
