using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Interoperability;

public class ManageInteroperabilityPage : BasePage
{
    public ManageInteroperabilityPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Manage integration types");

    public async Task OpenIntegrationTypeAsync(string integrationType)
    {
        await Page.GetByRole(AriaRole.Row, new() { Name = integrationType })
            .GetByRole(AriaRole.Link, new() { Name = "View" })
            .ClickAsync();
    }
}
