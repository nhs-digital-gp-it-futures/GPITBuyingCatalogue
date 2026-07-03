using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.EmailDomains;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Users;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Login;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin;

public class AdminPages
{
    private readonly ITestOutputHelper _output;
    private readonly AdminTestData _data;

    public LoginPage Login { get; }
    public AdminDashboardPage Dashboard { get; }
    public ManageUsersPage ManageUsers { get; }
    public AddUserPage AddUser { get; }
    public AllowedEmailDomainsPage AllowedEmailDomains { get; }
    public AddEmailDomainPage AddEmailDomain { get; }
    public DeleteEmailDomainPage DeleteEmailDomain { get; }

    public AdminPages(IPage page, ITestOutputHelper output, AdminTestData data)
    {
        _output = output;
        _data = data;

        Login = new LoginPage(page);
        Dashboard = new AdminDashboardPage(page);
        ManageUsers = new ManageUsersPage(page);
        AddUser = new AddUserPage(page);
        AllowedEmailDomains = new AllowedEmailDomainsPage(page);
        AddEmailDomain = new AddEmailDomainPage(page);
        DeleteEmailDomain = new DeleteEmailDomainPage(page);
    }

    public async Task LoginAsAdminAsync()
    {
        _output.WriteLine("Admin login");
        await Login.NavigateAsync(_data.BaseUrl);
        await Login.LoginAsync(_data.Email, _data.Password);
        await Login.AssertLoginSuccessfulAsync("Buying Catalogue admin");
    }

    public async Task CreateNewUserAsync(
        string organisation,
        string firstName,
        string lastName,
        string email,
        string accountType,
        string accountStatus)
    {
        _output.WriteLine($"Create new user: {firstName} {lastName}");
        await Dashboard.GoToManageUsersAsync();
        await ManageUsers.AssertOnPageAsync();
        await ManageUsers.GoToAddNewUserAsync();
        await AddUser.AssertOnPageAsync();
        await AddUser.AddUserAsync(organisation, firstName, lastName, email, accountType, accountStatus);
        await ManageUsers.AssertOnPageAsync();
        await ManageUsers.AssertUserExistsAsync(email);
    }

    public async Task CreateEmailDomainAsync(string domain)
    {
        _output.WriteLine($"Create email domain: {domain}");
        await Dashboard.GoToManageEmailDomainsAsync();
        await AllowedEmailDomains.AssertOnPageAsync();
        await AllowedEmailDomains.GoToAddEmailDomainAsync();
        await AddEmailDomain.AssertOnPageAsync();
        await AddEmailDomain.AddEmailDomainAsync(domain);
        await AllowedEmailDomains.AssertOnPageAsync();
    }

    public async Task DeleteEmailDomainAsync(string domain)
    {
        _output.WriteLine($"Delete email domain: {domain}");
        await AllowedEmailDomains.DeleteEmailDomainAsync(domain);
        await DeleteEmailDomain.AssertOnPageAsync();
        await DeleteEmailDomain.ConfirmDeleteAsync();
        await AllowedEmailDomains.AssertOnPageAsync();
    }
}
