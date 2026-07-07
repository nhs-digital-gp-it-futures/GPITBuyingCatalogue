using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin;

public class AdminDashboardPage : BasePage
{
    private ILocator ManageUsersLink => Page.GetByRole(AriaRole.Link, new() { Name = "Manage all users" });
    private ILocator ManageEmailDomainsLink => Page.GetByRole(AriaRole.Link, new() { Name = "Manage allowed email domains" });

    public AdminDashboardPage(IPage page) : base(page) { }

    public async Task GoToManageUsersAsync() => await ManageUsersLink.ClickAsync();

    public async Task GoToManageEmailDomainsAsync() => await ManageEmailDomainsLink.ClickAsync();
}
