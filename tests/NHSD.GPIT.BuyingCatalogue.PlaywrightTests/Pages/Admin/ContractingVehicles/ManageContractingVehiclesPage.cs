using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.ContractingVehicles;

public class ManageContractingVehiclesPage : BasePage
{
    private ILocator AddNewLink => Page.GetByRole(AriaRole.Link, new() { Name = "Add a new contracting vehicle" });

    public ManageContractingVehiclesPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Manage contracting vehicles");

    public async Task GoToAddNewAsync() => await AddNewLink.ClickAsync();

    public async Task AssertContractingVehicleExistsAsync(string name)
    {
        var row = Page.GetByRole(AriaRole.Row).Filter(new() { HasText = name });
        await row.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
    }
}
