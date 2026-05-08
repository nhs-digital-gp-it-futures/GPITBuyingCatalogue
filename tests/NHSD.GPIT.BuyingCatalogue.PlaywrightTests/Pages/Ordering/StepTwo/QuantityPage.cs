using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepTwo;

public class QuantityPage : BasePage
{
    private ILocator StartLink => Page.GetByRole(AriaRole.Link, new() { Name = "Start" });
    private ILocator SelectLink => Page.GetByRole(AriaRole.Link, new() { Name = "Select" });

    public QuantityPage(IPage page) : base(page) { }

    public async Task EnterQuantitiesAsync(Dictionary<string, string> practiceQuantities)
    {
        await AssertHeadingAsync("Catalogue solution and services");
        await StartLink.ClickAsync();

        await AssertHeadingAsync("Quantity of catalogue solution");
        await SelectLink.ClickAsync();

        foreach (var (practice, quantity) in practiceQuantities)
            await Page.GetByRole(AriaRole.Row, new() { Name = practice })
                .GetByLabel("Patient total")
                .FillAsync(quantity);

        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Quantity of catalogue solution");
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Confirm quantities");
        await ClickContinueLinkAsync();

        await AssertHeadingAsync("Edit solutions and services");
        await ClickSaveAndContinueLinkAsync();
    }
}
