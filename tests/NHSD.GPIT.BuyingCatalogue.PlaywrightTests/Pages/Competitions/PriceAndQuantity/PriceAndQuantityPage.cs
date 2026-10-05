using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.PriceAndQuantity;

public class PriceAndQuantityPage : BasePage
{
    public PriceAndQuantityPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() =>
        await AssertHeadingAsync("Price and quantity");

    public async Task SaveAndContinueLinkAsync() =>
        await ClickSaveAndContinueLinkAsync();
    
    public async Task SetPriceAsync(CompetitionServiceType type, string priceOption = "")
    {
        await Page.Locator(type.ContainerId())
            .GetByRole(AriaRole.Row, new() { Name = "Price" })
            .GetByRole(AriaRole.Link, new() { Name = "Start", Exact = true })
            .ClickAsync();

        if (!string.IsNullOrWhiteSpace(priceOption))
        {
            await Page.GetByLabel(priceOption).CheckAsync();
            await ClickSaveAndContinueAsync();
        }

        await AssertHeadingAsync(type.PriceHeading());
        await ClickSaveAndContinueAsync();
    }

    public async Task SetQuantityAsync(
        CompetitionServiceType type,
        string bankfieldTotal,
        string beechwoodTotal)
    {
        await Page.Locator(type.ContainerId())
            .GetByRole(AriaRole.Row, new() { Name = "Quantity" })
            .GetByRole(AriaRole.Link, new() { Name = "Start", Exact = true })
            .ClickAsync();

        await AssertHeadingAsync(type.QuantityHeading());
        await Page.GetByRole(AriaRole.Link, new() { Name = "Select" }).ClickAsync();

        if (type == CompetitionServiceType.CatalogueSolution)
        {
            await AssertHeadingAsync("Review patient list sizes");
            await Page.GetByRole(AriaRole.Row, new() { Name = "BANKFIELD SURGERY B84016" })
                .GetByLabel("Patient total").FillAsync(bankfieldTotal);
            await Page.GetByRole(AriaRole.Row, new() { Name = "BEECHWOOD MEDICAL CENTRE" })
                .GetByLabel("Patient total").FillAsync(beechwoodTotal);
        }
        else
        {
            await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "BANKFIELD SURGERY" })
                .GetByRole(AriaRole.Textbox).FillAsync(bankfieldTotal);
            await Page.GetByRole(AriaRole.Row).Filter(new() { HasText = "BEECHWOOD MEDICAL CENTRE" })
                .GetByRole(AriaRole.Textbox).FillAsync(beechwoodTotal);
        }

        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync(type.QuantityHeading());
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Confirm quantities");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Continue" }).ClickAsync();
    }
}
