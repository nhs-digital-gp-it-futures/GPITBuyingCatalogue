using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepOne;

public class SupplierPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Supplier information and" });
    private ILocator SupplierInput => Page.Locator("input[role='combobox']:visible");
    private ILocator SuggestionsList => Page.Locator("ul[role='listbox']:visible");
    private ILocator YesRadio => Page.GetByLabel("Yes");
    private ILocator ConfirmButton => Page.GetByRole(AriaRole.Button, new() { Name = "Confirm supplier" });

    public SupplierPage(IPage page) : base(page) { }

    public async Task NavigateAsync() =>
        await NavigationLink.ClickAsync();

    public async Task SearchAndSelectSupplierAsync(string supplierName)
    {
        await SupplierInput.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await SupplierInput.ClickAsync();
        await SupplierInput.FillAsync(string.Empty);
        await SupplierInput.PressSequentiallyAsync(supplierName);

        await SuggestionsList.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await SuggestionsList.GetByText(supplierName, new() { Exact = true }).ClickAsync();

        await ClickSaveAndContinueAsync();
    }

    public async Task SelectSupplierByRadioAsync(string supplierName)
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = supplierName }).CheckAsync();
        await ClickSaveAndContinueAsync();
    }

    public async Task ConfirmSupplierAsync()
    {
        await YesRadio.CheckAsync();
        await ConfirmButton.ClickAsync();
        await AssertHeadingAsync("Supplier contact details");
        await ClickSaveAndContinueAsync();
    }
}
