using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Infrastructure;
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
        await orderPages.Login.NavigateAsync(Fixture.BaseUrl);
        var result = await orderPages.Login.RunAccessibilityScanAsync();

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
        var result = await orderPages.OrderType.RunAccessibilityScanAsync();

        AccessibilityReporter.Report(result, Output, "What do you want to order? (order type page)");

        var blocking = result.Violations
            .Where(v => v.Impact == "critical" || v.Impact == "serious")
            .ToList();

        Assert.True(
            blocking.Count == 0,
            $"Login page has {blocking.Count} critical or serious accessibility violations. See test output for detail.");
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task UploadServiceRecipientsPage_AccessibilityScan()
    {
        await orderPages.GoToUploadServiceRecipientsPageAsync(SolutionName);
        var result = await orderPages.ServiceRecipients.RunAccessibilityScanAsync();

        AccessibilityReporter.Report(result, Output, "Upload service recipients page");

        var blocking = result.Violations
            .Where(v => v.Impact == "critical" || v.Impact == "serious")
            .ToList();

        Assert.True(
            blocking.Count == 0,
            $"Login page has {blocking.Count} critical or serious accessibility violations. See test output for detail.");
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task AddServiceRecipientsPage_AccessibilityScan()
    {
        await orderPages.GoToAddServiceRecipientsPageAsync();
        var result = await orderPages.ServiceRecipients.RunAccessibilityScanAsync();

        AccessibilityReporter.Report(result, Output, "Add service recipients page");

        var blocking = result.Violations
            .Where(v => v.Impact == "critical" || v.Impact == "serious")
            .ToList();

        Assert.True(
            blocking.Count == 0,
            $"Login page has {blocking.Count} critical or serious accessibility violations. See test output for detail.");
    }
}
