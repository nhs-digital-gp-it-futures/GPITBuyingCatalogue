using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Suppliers;

public class SupplierContactsPage : BasePage
{
    private ILocator AddContactLink => Page.GetByRole(AriaRole.Link, new() { Name = "Add a contact" });

    public SupplierContactsPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Supplier contacts");

    public async Task GoToAddContactAsync() => await AddContactLink.ClickAsync();

    public async Task ContinueAsync() => await ClickSaveAndContinueLinkAsync();
}
