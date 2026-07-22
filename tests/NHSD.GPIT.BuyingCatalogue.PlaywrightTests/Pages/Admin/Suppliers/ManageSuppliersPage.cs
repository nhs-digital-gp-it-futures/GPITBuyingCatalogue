using System.Threading.Tasks;
using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Suppliers;

public class ManageSuppliersPage : BasePage
{
    private ILocator AddSupplierLink => Page.GetByRole(AriaRole.Link, new() { Name = "Add a supplier" });

    public ManageSuppliersPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Manage suppliers");

    public async Task GoToAddSupplierAsync() => await AddSupplierLink.ClickAsync();

    public async Task AssertSupplierExistsAsync(string name)
    {
        var row = Page.GetByRole(AriaRole.Row).Filter(new() { HasText = name });
        await row.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
    }
}
