using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.TestData;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;

[Collection("Playwright")]
public abstract class BaseTest : IAsyncLifetime
{
    protected readonly TestServerFixture Fixture;
    protected readonly ITestOutputHelper Output;
    protected IPage Page = null!;
    protected OrderingPages orderPages = null!;

    private readonly TestSettings _settings;
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private IBrowserContext _context = null!;
    private string _testName = string.Empty;

    protected BaseTest(TestServerFixture fixture, ITestOutputHelper output)
    {
        Fixture = fixture;
        Output = output;
        _settings = TestSettings.FromEnvironment();
    }

    public async Task InitializeAsync()
    {
        _testName = $"{GetType().Name}_{DateTime.UtcNow:yyyyMMdd_HHmmss}";
        _playwright = await Playwright.CreateAsync();
        _browser = await CreateBrowserAsync();
        _context = await CreateContextAsync();

        Page = await _context.NewPageAsync();

        var data = new OrderTestData { BaseUrl = Fixture.BaseUrl };
        //Pages = new OrderingPages(Page, Output, new OrderTestData());
        orderPages = new OrderingPages(Page, Output, new OrderTestData());

        Output.WriteLine($"Test started: {_testName}");
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _browser.DisposeAsync();
        _playwright.Dispose();
    }

    private async Task<IBrowser> CreateBrowserAsync() =>
        await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = _settings.Headless
        });

    private async Task<IBrowserContext> CreateContextAsync() =>
        await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = _settings.IgnoreHttpsErrors
        });
}
