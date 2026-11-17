using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepTwo;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.Amend;

public class AmendItemsPage : BasePage
{
    private ILocator SelectLink => Page.GetByRole(AriaRole.Link, new() { Name = "Select" });

    public AmendItemsPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() =>
        await AssertHeadingAsync("Amend items from the previous order");

    // Change quantities on an item already in the order
    public async Task AmendQuantitiesAsync(string itemName, string quantityHeading, Dictionary<string, string> practiceQuantities)
    {
        await AssertOnPageAsync();
        await ItemCard(itemName)
            .GetByRole(AriaRole.Row, new() { Name = "Quantity" })
            .GetByRole(AriaRole.Link, new() { Name = "Start", Exact = true })
            .ClickAsync();

        await EnterQuantitiesAsync(quantityHeading, practiceQuantities);
    }

    // Add a new additional or associated service, price it, then enter its quantities
    public async Task AddOnServiceAsync(AddOnServiceType addOn, string serviceName, Dictionary<string, string> practiceQuantities)
    {
        await AssertOnPageAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = addOn.LinkText() }).ClickAsync();
        await AssertHeadingAsync(addOn.AddHeading());
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = serviceName }).CheckAsync();
        await ClickSaveAndContinueAsync();

        await AssertOnPageAsync();
        await ItemCard(serviceName)
            .GetByRole(AriaRole.Link, new() { Name = "Start", Exact = true })
            .First
            .ClickAsync();
        await AssertHeadingAsync(addOn.PriceHeading());
        await ClickSaveAndContinueAsync();

        await AmendQuantitiesAsync(serviceName, addOn.QuantityHeading(), practiceQuantities);
    }

    public async Task SaveAndContinueAsync()
    {
        await AssertOnPageAsync();
        await ClickSaveAndContinueAsync();
    }

    private ILocator ItemCard(string itemName) =>
        Page.Locator(".nhsuk-card", new()
        {
            Has = Page.GetByRole(AriaRole.Heading, new() { Name = itemName, Exact = true })
        });

    private async Task EnterQuantitiesAsync(string quantityHeading, Dictionary<string, string> practiceQuantities)
    {
        await AssertHeadingAsync(quantityHeading);
        await SelectLink.ClickAsync();

        foreach (var (practice, quantity) in practiceQuantities)
            await Page.GetByRole(AriaRole.Row)
                .Filter(new() { HasText = practice })
                .GetByRole(AriaRole.Textbox)
                .FillAsync(quantity);

        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync(quantityHeading);
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Confirm quantities");
        await ClickContinueLinkAsync();
    }
}
