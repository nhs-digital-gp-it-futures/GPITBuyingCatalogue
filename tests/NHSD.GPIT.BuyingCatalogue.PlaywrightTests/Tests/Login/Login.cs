using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData.Builders;
using static Microsoft.Playwright.Assertions;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Tests.Login;

public class LoginTests : BaseTest
{
    public LoginTests(TestServerFixture fixture, ITestOutputHelper output)
        : base(fixture, output)
    {
    }

    [Fact]
    [Trait("Category", Categories.Smoke)]
    public async Task UserCanLoginSuccessfully()
    {
        var data = new OrderTestDataBuilder()
            .WithBaseUrl(Fixture.BaseUrl)
            .Build();

        await orderPages.LoginAsync();
    }
}
