using System.Linq;
using System.Threading.Tasks;
using Deque.AxeCore.Commons;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Tests.Accessibility;

[Collection("Playwright")]
public class AccessibilityTests : BaseTest
{
    private const string SolutionName = "Emis Web GP";

    public AccessibilityTests(TestServerFixture fixture, ITestOutputHelper output)
        : base(fixture, output) { }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task LoginPage_MeetsWcagStandards()
    {
        // Navigate to the login page
        await orderPages.Login.NavigateAsync(Fixture.BaseUrl);

        // Run the WCAG-scoped axe scan
        var result = await orderPages.Login.RunAccessibilityScanAsync();

        // Print a readable report to the test output
        AccessibilityReporter.Report(result, Output, "Login page");

        var blocking = result.Violations
            .Where(v => v.Impact == "critical" || v.Impact == "serious")
            .ToList();

        Assert.True(
            blocking.Count == 0,
            $"Login page has {blocking.Count} critical or serious accessibility violations. See test output for detail.");
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task OrderTypePage_AccessibilityScan()
    {
        await orderPages.LoginAsync();
        await orderPages.GoToOrderTypePageAsync();
        
        var result = await orderPages.OrderType.RunAccessibilityScanAsync();

        // Print the full report for the POC comparison against WAVE
        AccessibilityReporter.Report(result, Output, "What do you want to order? (order type page)");

        var blocking = result.Violations
            .Where(v => v.Impact == "critical" || v.Impact == "serious")
            .ToList();

        Assert.True(
            blocking.Count == 0,
            $"Login page has {blocking.Count} critical or serious accessibility violations. See test output for detail.");
    }

    [Fact]
    [Trait("Category", Categories.Regression)]
    public async Task DeclarationPage_HasMeaningfulPageTitle()
    {
        await orderPages.GoToDeclarationPageAsync(SolutionName);
        //await orderPages.AssertPageHasMeaningfulTitleAsync();

        var result = await orderPages.OrderType.RunAccessibilityScanAsync();

        // Print the full report for the POC comparison against WAVE
        AccessibilityReporter.Report(result, Output, "What do you want to order? (order type page)");

        var blocking = result.Violations
            .Where(v => v.Impact == "critical" || v.Impact == "serious")
            .ToList();

        Assert.True(
            blocking.Count == 0,
            $"Login page has {blocking.Count} critical or serious accessibility violations. See test output for detail.");
    }
}
