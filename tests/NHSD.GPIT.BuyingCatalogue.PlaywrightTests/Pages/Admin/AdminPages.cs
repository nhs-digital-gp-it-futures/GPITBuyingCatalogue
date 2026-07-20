using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.ContractingVehicles;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.EmailDomains;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Interoperability;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.SupplierDefinedEpics;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Suppliers;
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
    public ManageContractingVehiclesPage ManageContractingVehicles { get; }
    public AddContractingVehiclePage AddContractingVehicle { get; }
    public SupplierDefinedEpicsPage SupplierDefinedEpics { get; }
    public AddSupplierDefinedEpicCapabilitiesPage AddSupplierDefinedEpicCaps { get; }
    public AddSupplierDefinedEpicDetailsPage AddSupplierDefinedEpicDetails { get; }
    public ManageInteroperabilityPage ManageInteroperability { get; }
    public IntegrationTypePage IntegrationType { get; }
    public AddIntegrationTypePage AddIntegrationType { get; }
    public ManageSuppliersPage ManageSuppliers { get; }
    public AddSupplierDetailsPage AddSupplierDetails { get; }
    public SupplierInformationPage SupplierInformation { get; }
    public SupplierAddressPage SupplierAddress { get; }
    public SupplierContactsPage SupplierContacts { get; }
    public AddSupplierContactPage AddSupplierContact { get; }
    public SupplierStatusPage SupplierStatus { get; }

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
        ManageContractingVehicles = new ManageContractingVehiclesPage(page);
        AddContractingVehicle = new AddContractingVehiclePage(page);
        SupplierDefinedEpics = new SupplierDefinedEpicsPage(page);
        AddSupplierDefinedEpicCaps = new AddSupplierDefinedEpicCapabilitiesPage(page);
        AddSupplierDefinedEpicDetails = new AddSupplierDefinedEpicDetailsPage(page);
        ManageInteroperability = new ManageInteroperabilityPage(page);
        IntegrationType = new IntegrationTypePage(page);
        AddIntegrationType = new AddIntegrationTypePage(page);
        ManageSuppliers = new ManageSuppliersPage(page);
        AddSupplierDetails = new AddSupplierDetailsPage(page);
        SupplierInformation = new SupplierInformationPage(page);
        SupplierAddress = new SupplierAddressPage(page);
        SupplierContacts = new SupplierContactsPage(page);
        AddSupplierContact = new AddSupplierContactPage(page);
        SupplierStatus = new SupplierStatusPage(page);
    }

    public async Task LoginAsAdminAsync()
    {
        _output.WriteLine("Admin login");
        await Login.NavigateAsync(_data.BaseUrl);
        await Login.LoginAsync(_data.Email, _data.Password);
        await Login.AssertLoginSuccessfulAsync("Buying Catalogue admin");
    }

    public async Task CreateNewUserAsync(
        string organisation, string firstName, string lastName,
        string email, string accountType, string accountStatus)
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

    public async Task CreateContractingVehicleAsync(string name, string maxDuration, string[] fundingTypes)
    {
        _output.WriteLine($"Create contracting vehicle: {name}");
        await Dashboard.GoToManageContractingVehiclesAsync();
        await ManageContractingVehicles.AssertOnPageAsync();
        await ManageContractingVehicles.GoToAddNewAsync();
        await AddContractingVehicle.AssertOnPageAsync();
        await AddContractingVehicle.AddAsync(name, maxDuration, fundingTypes);
        await ManageContractingVehicles.AssertOnPageAsync();
        await ManageContractingVehicles.AssertContractingVehicleExistsAsync(name);
    }

    public async Task CreateSupplierDefinedEpicAsync(string name, string description, string capability)
    {
        _output.WriteLine($"Create supplier defined Epic: {name}");
        await Dashboard.GoToManageSupplierDefinedEpicsAsync();
        await SupplierDefinedEpics.AssertOnPageAsync();
        await SupplierDefinedEpics.GoToAddNewAsync();
        await AddSupplierDefinedEpicCaps.AssertOnPageAsync();
        await AddSupplierDefinedEpicCaps.SelectCapabilityAndContinueAsync(capability);
        await AddSupplierDefinedEpicDetails.AssertOnPageAsync();
        await AddSupplierDefinedEpicDetails.AddAsync(name, description);
        await SupplierDefinedEpics.AssertOnPageAsync();
        await SupplierDefinedEpics.AssertEpicExistsAsync(name);
    }

    public async Task GoToManageInteroperabilityAsync()
    {
        _output.WriteLine("Go to manage interoperability");
        await Dashboard.GoToManageInteroperabilityAsync();
        await ManageInteroperability.AssertOnPageAsync();
    }

    public async Task AddIntegrationTypeAsync(string integrationType, string name, string description = "")
    {
        _output.WriteLine($"Add integration type to {integrationType}: {name}");
        await ManageInteroperability.OpenIntegrationTypeAsync(integrationType);
        await IntegrationType.AssertOnPageAsync(integrationType);
        await IntegrationType.GoToAddNewAsync();
        await AddIntegrationType.AssertOnPageAsync();
        await AddIntegrationType.AddAsync(name, description);
        await IntegrationType.AssertOnPageAsync(integrationType);
        await IntegrationType.AssertIntegrationTypeExistsAsync(name);
        await IntegrationType.GoBackAsync();
    }

    public async Task CreateSupplierAsync(
        string name, string legalName, string about,
        SupplierAddress address, SupplierContact contact)
    {
        _output.WriteLine($"Create supplier: {name}");
        await Dashboard.GoToManageSuppliersAsync();
        await ManageSuppliers.GoToAddSupplierAsync();
        await AddSupplierDetails.AssertOnPageAsync();
        await AddSupplierDetails.AddAsync(name, legalName, about);

        await SupplierInformation.GoToEditAddressAsync();
        await SupplierAddress.AssertOnPageAsync();
        await SupplierAddress.AddAsync(address);
        await SupplierInformation.AssertOnPageAsync();

        await SupplierInformation.GoToEditContactsAsync();
        await SupplierContacts.AssertOnPageAsync();
        await SupplierContacts.GoToAddContactAsync();
        await AddSupplierContact.AddAsync(contact);
        await SupplierContacts.AssertOnPageAsync();
        await SupplierContacts.ContinueAsync();

        await SupplierStatus.SetStatusAndSaveAsync("Active");
        await ManageSuppliers.AssertOnPageAsync();
        await ManageSuppliers.AssertSupplierExistsAsync(name);
    }
}
