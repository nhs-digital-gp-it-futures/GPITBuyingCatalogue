using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Suppliers;

public class SupplierStatusPage : BasePage
{
    public SupplierStatusPage(IPage page) : base(page) { }

    public async Task SetStatusAndSaveAsync(string status)
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = status, Exact = true }).CheckAsync();
        await ClickSaveAndContinueAsync();
    }
}
