using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.SupplierDefinedEpics;

public class AddSupplierDefinedEpicCapabilitiesPage : BasePage
{
    private ILocator ApplyCapabilitiesButton => Page.GetByRole(AriaRole.Button, new() { Name = "Apply Capabilities" });

    public AddSupplierDefinedEpicCapabilitiesPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Capabilities for this supplier defined Epic");

    public async Task SelectCapabilityAndContinueAsync(string capability)
    {
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = capability }).CheckAsync();
        await ApplyCapabilitiesButton.ClickAsync();
    }
}
