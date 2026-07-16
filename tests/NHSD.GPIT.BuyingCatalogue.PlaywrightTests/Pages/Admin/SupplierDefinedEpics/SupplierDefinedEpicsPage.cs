using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.SupplierDefinedEpics;

public class SupplierDefinedEpicsPage : BasePage
{
    private ILocator AddNewLink => Page.GetByRole(AriaRole.Link, new() { Name = "Add a supplier defined Epic" });

    public SupplierDefinedEpicsPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Supplier defined Epics");

    public async Task GoToAddNewAsync() => await AddNewLink.ClickAsync();

    public async Task AssertEpicExistsAsync(string name)
    {
        var row = Page.GetByRole(AriaRole.Row).Filter(new() { HasText = name });
        await row.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
    }
}
