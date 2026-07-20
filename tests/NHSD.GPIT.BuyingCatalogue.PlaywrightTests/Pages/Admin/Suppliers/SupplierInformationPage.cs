using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Suppliers;

public class SupplierInformationPage : BasePage
{
    private ILocator EditAddressLink => Page.Locator("#EditSupplierAddressLink");
    private ILocator EditContactsLink => Page.Locator("#EditSupplierContactsLink");

    public SupplierInformationPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Supplier information");

    public async Task GoToEditAddressAsync() => await EditAddressLink.ClickAsync();

    public async Task GoToEditContactsAsync() => await EditContactsLink.ClickAsync();
}
