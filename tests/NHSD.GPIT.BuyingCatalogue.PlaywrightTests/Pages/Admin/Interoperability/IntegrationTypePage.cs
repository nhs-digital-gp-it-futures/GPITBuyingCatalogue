using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Interoperability;

public class IntegrationTypePage : BasePage
{
    private ILocator AddNewLink => Page.GetByRole(AriaRole.Link, new() { Name = "Add a new integration type" });
    private ILocator GoBackLink => Page.GetByRole(AriaRole.Link, new() { Name = "Go back" });

    public IntegrationTypePage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync(string integrationType) =>
        await AssertHeadingAsync($"{integrationType} Integration types");

    public async Task GoToAddNewAsync() => await AddNewLink.ClickAsync();

    public async Task GoBackAsync() => await GoBackLink.ClickAsync();

    public async Task AssertIntegrationTypeExistsAsync(string name)
    {
        var row = Page.GetByRole(AriaRole.Row).Filter(new() { HasText = name });
        await row.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
    }
}
