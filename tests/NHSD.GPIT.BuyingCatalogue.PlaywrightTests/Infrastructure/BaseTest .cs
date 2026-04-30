using System.Threading.Tasks;
using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;

[Collection("Playwright")]
public abstract class BaseTest : IAsyncLifetime
{
    protected readonly TestServerFixture Fixture;
    protected IPage Page = null!;

    private readonly TestSettings _settings;
    private IPlaywright _playwright = null!;
    private IBrowser _browser = null!;
    private IBrowserContext _context = null!;

    protected BaseTest(TestServerFixture fixture)
    {
        Fixture = fixture;
        _settings = TestSettings.FromEnvironment();
    }

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await CreateBrowserAsync();
        _context = await CreateContextAsync();
        Page = await _context.NewPageAsync();
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
