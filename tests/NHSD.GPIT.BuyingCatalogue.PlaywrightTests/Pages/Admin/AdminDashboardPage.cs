using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin;

public class AdminDashboardPage : BasePage
{
    private ILocator ManageUsersLink => Page.GetByRole(AriaRole.Link, new() { Name = "Manage all users" });
    private ILocator ManageEmailDomainsLink => Page.GetByRole(AriaRole.Link, new() { Name = "Manage allowed email domains" });
    private ILocator ManageContractingVehiclesLink => Page.GetByRole(AriaRole.Link, new() { Name = "Manage contracting vehicles" });
    private ILocator ManageSupplierDefinedEpicsLink => Page.GetByRole(AriaRole.Link, new() { Name = "Manage supplier defined Epics" });
    private ILocator ManageInteroperabilityLink => Page.GetByRole(AriaRole.Link, new() { Name = "Manage interoperability" });
    private ILocator ManageSuppliersLink => Page.GetByRole(AriaRole.Link, new() { Name = "Manage supplier organisations" });

    public AdminDashboardPage(IPage page) : base(page) { }

    public async Task GoToManageUsersAsync() => await ManageUsersLink.ClickAsync();
    public async Task GoToManageEmailDomainsAsync() => await ManageEmailDomainsLink.ClickAsync();
    public async Task GoToManageContractingVehiclesAsync() => await ManageContractingVehiclesLink.ClickAsync();
    public async Task GoToManageSupplierDefinedEpicsAsync() => await ManageSupplierDefinedEpicsLink.ClickAsync();
    public async Task GoToManageInteroperabilityAsync() => await ManageInteroperabilityLink.ClickAsync();
    public async Task GoToManageSuppliersAsync() => await ManageSuppliersLink.ClickAsync();
}
