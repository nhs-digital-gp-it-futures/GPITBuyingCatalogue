using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepTwo;

public class QuantityPage : BasePage
{
    private ILocator StartLink => Page.GetByRole(AriaRole.Link, new() { Name = "Start" });
    private ILocator SelectLink => Page.GetByRole(AriaRole.Link, new() { Name = "Select" });

    public QuantityPage(IPage page) : base(page) { }

    public async Task EnterQuantitiesAsync(
    Dictionary<string, string> practiceQuantities,
    bool completeEdit = true,
    bool stopOnConfirm = false,
    bool stopOnQuantityPage = false)
    {
        await AssertHeadingAsync("Catalogue solution and services");
        await StartLink.ClickAsync();

        await AssertHeadingAsync("Quantity of catalogue solution");

        if (stopOnQuantityPage)
            return;

        await SelectLink.ClickAsync();

        foreach (var (practice, quantity) in practiceQuantities)
            await Page.GetByRole(AriaRole.Row)
                .Filter(new() { HasText = practice })
                .GetByRole(AriaRole.Textbox)
                .FillAsync(quantity);

        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Quantity of catalogue solution");
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Confirm quantities");

        if (stopOnConfirm)
            return;

        await ClickContinueLinkAsync();

        if (completeEdit)
        {
            await AssertHeadingAsync("Edit solutions and services");
            await ClickSaveAndContinueLinkAsync();
        }
    }

    // Generic add-on quantity entry. completeEdit = false leaves the Edit page open.
    public async Task EnterAddOnQuantitiesAsync(
        Dictionary<string, string> practiceQuantities,
        string quantityHeading,
        bool completeEdit = true)
    {
        await AssertHeadingAsync("Catalogue solution and services");
        await StartLink.ClickAsync();
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

        if (completeEdit)
        {
            await AssertHeadingAsync("Edit solutions and services");
            await ClickSaveAndContinueLinkAsync();
        }
    }

    // Associated Service Only journey quantities
    public async Task EnterAssociatedServiceQuantitiesAsync(Dictionary<string, string> practiceQuantities)
    {
        await AssertHeadingAsync("Catalogue solution and services");
        await StartLink.ClickAsync();
        await AssertHeadingAsync("Quantity of associated service");
        await SelectLink.ClickAsync();

        foreach (var (practice, quantity) in practiceQuantities)
            await Page.GetByRole(AriaRole.Row)
                .Filter(new() { HasText = practice })
                .GetByRole(AriaRole.Textbox)
                .FillAsync(quantity);

        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Quantity of associated service");
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Confirm quantities");
        await ClickContinueLinkAsync();
        await AssertHeadingAsync("Edit solutions and services");
        await ClickSaveAndContinueLinkAsync();
    }
}
