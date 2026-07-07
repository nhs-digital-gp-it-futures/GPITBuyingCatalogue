using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Tests.Admin;

public class AdminTests : BaseTest
{
    public AdminTests(TestServerFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

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
}
