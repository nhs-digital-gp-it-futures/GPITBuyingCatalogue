using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Suppliers;

public class SupplierAddressPage : BasePage
{
    private ILocator Line1Input => Page.GetByRole(AriaRole.Textbox, new() { Name = "Address Line 1" });
    private ILocator Line2Input => Page.GetByRole(AriaRole.Textbox, new() { Name = "Address Line 2 (optional)" });
    private ILocator Line3Input => Page.GetByRole(AriaRole.Textbox, new() { Name = "Address Line 3 (optional)" });
    private ILocator TownInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Town or city" });
    private ILocator PostcodeInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Postcode" });
    private ILocator CountryInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Country" });

    public SupplierAddressPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Supplier registered address");

    public async Task AddAsync(SupplierAddress address)
    {
        await Line1Input.FillAsync(address.Line1);
        await Line2Input.FillAsync(address.Line2);
        await Line3Input.FillAsync(address.Line3);
        await TownInput.FillAsync(address.Town);
        await PostcodeInput.FillAsync(address.Postcode);
        await CountryInput.FillAsync(address.Country);
        await ClickSaveAndContinueAsync();
    }
}
