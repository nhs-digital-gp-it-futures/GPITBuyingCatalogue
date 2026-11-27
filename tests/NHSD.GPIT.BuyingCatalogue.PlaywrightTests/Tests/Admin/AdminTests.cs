using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Tests.Admin;

public class AdminTests : BaseTest
{
    private static readonly string[] ContractingVehicleFundingTypes =
    [
        "GPIT funding",
        "Local funding"
    ];

    public AdminTests(TestServerFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    private Task LoginAsync(AdminUserType userType) =>
        userType == AdminUserType.Onboarding
            ? AdminPages.LoginAsOnboardingUserAsync()
            : AdminPages.LoginAsAdminAsync();

    [Fact]
    [Trait("Category", Categories.Admin)]
    public async Task AdminCreateNewUser()
    {
        var data = new AdminTestData();

        await AdminPages.LoginAsAdminAsync();
        await AdminPages.CreateNewUserAsync(
            organisation: data.Organisation,
            firstName: data.FirstName,
            lastName: data.LastName,
            email: data.UserEmail,
            accountType: data.AccountType,
            accountStatus: data.AccountStatus);
    }

    [Fact]
    [Trait("Category", Categories.Admin)]
    public async Task AdminCreateAndDeleteEmailDomain()
    {
        var data = new AdminTestData();

        await AdminPages.LoginAsAdminAsync();
        await AdminPages.CreateEmailDomainAsync(data.EmailDomain);
        await AdminPages.DeleteEmailDomainAsync(data.EmailDomain);
    }

    [Theory]
    [InlineData(AdminUserType.Admin)]
    [InlineData(AdminUserType.Onboarding)]
    [Trait("Category", Categories.Admin)]
    public async Task AdminAddNewContractingVehicle(AdminUserType userType)
    {
        var data = new AdminTestData();

        await LoginAsync(userType);
        await AdminPages.CreateContractingVehicleAsync(
            name: data.ContractingVehicleName,
            maxDuration: data.ContractingVehicleDuration,
            fundingTypes: ContractingVehicleFundingTypes);
    }

    [Theory]
    [InlineData(AdminUserType.Admin)]
    [InlineData(AdminUserType.Onboarding)]
    [Trait("Category", Categories.Admin)]
    public async Task AdminAddSupplierDefinedEpic(AdminUserType userType)
    {
        var data = new AdminTestData();

        await LoginAsync(userType);
        await AdminPages.CreateSupplierDefinedEpicAsync(
            name: data.EpicName,
            description: data.EpicDescription,
            capability: data.EpicCapability);
    }

    [Fact]
    [Trait("Category", Categories.Admin)]
    public async Task AdminManageInteroperability()
    {
        var data = new AdminTestData();

        await AdminPages.LoginAsAdminAsync();
        await AdminPages.GoToManageInteroperabilityAsync();

        await AdminPages.AddIntegrationTypeAsync("IM1", $"IM1 {data.IntegrationTypeName}");
        await AdminPages.AddIntegrationTypeAsync("GP Connect", $"GP Connect {data.IntegrationTypeName}");
        await AdminPages.AddIntegrationTypeAsync("NHS App", $"NHS App {data.IntegrationTypeName}", data.IntegrationTypeDescription);
    }

    [Theory]
    [InlineData(AdminUserType.Admin)]
    [InlineData(AdminUserType.Onboarding)]
    [Trait("Category", Categories.Admin)]
    public async Task AdminAddNewSupplier(AdminUserType userType)
    {
        var data = new AdminTestData();

        var address = new SupplierAddress(
            data.SupplierAddressLine1, data.SupplierAddressLine2, data.SupplierAddressLine3,
            data.SupplierTown, data.SupplierPostcode, data.SupplierCountry);

        var contact = new SupplierContact(
            data.SupplierContactFirstName, data.SupplierContactLastName, data.SupplierContactDepartment,
            data.SupplierContactPhone, data.SupplierContactEmail);

        await LoginAsync(userType);
        await AdminPages.CreateSupplierAsync(
            name: data.SupplierName,
            legalName: data.SupplierLegalName,
            about: data.SupplierAbout,
            address: address,
            contact: contact);
    }

    [Theory]
    [InlineData(AdminUserType.Admin)]
    [InlineData(AdminUserType.Onboarding)]
    [Trait("Category", Categories.Admin)]
    public async Task AdminAddSolution(AdminUserType userType)
    {
        var data = new AdminTestData();

        await LoginAsync(userType);
        await AdminPages.CreateSolutionAsync(
            data: data,
            supplierValue: "10000",
            framework: "Tech Innovation Framework",
            supplierContactName: "Jonas Chan");
    }
}
