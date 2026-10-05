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
    public async Task LoginPage_AccessibilityScan()
    {
        await OrderPages.Login.NavigateAsync(Fixture.BaseUrl);
        var result = await OrderPages.Login.RunAccessibilityScanAsync();

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
        await OrderPages.LoginAsync();
        await OrderPages.GoToOrderTypePageAsync();
        var result = await OrderPages.OrderType.RunAccessibilityScanAsync();

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
    public async Task DeclarationPage_HasPageTitle()
    {
        await OrderPages.GoToDeclarationPageAsync(SolutionName);
        await OrderPages.Declaration.AssertPageHasTitleAsync();
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task OrderCompletedPage_HasPageTitle()
    {
        await OrderPages.GoToOrderCompletedPageAsync(SolutionName);
        await OrderPages.ReviewOrder.AssertPageHasTitleAsync();
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task UploadServiceRecipientsPage_AccessibilityScan()
    {
        await OrderPages.GoToUploadServiceRecipientsPageAsync(SolutionName);
        var result = await OrderPages.ServiceRecipients.RunAccessibilityScanAsync();

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
        await OrderPages.GoToAddServiceRecipientsPageAsync();
        var result = await OrderPages.ServiceRecipients.RunAccessibilityScanAsync();

        AccessibilityReporter.Report(result, Output, "Add service recipients page");

        var blocking = result.Violations
            .Where(v => v.Impact == "critical" || v.Impact == "serious")
            .ToList();

        Assert.True(
            blocking.Count == 0,
            $"Login page has {blocking.Count} critical or serious accessibility violations. See test output for detail.");
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task ReviewPlannedDeliveryDatesPage_AccessibilityScan()
    {
        await OrderPages.GoToReviewPlannedDeliveryDatesPageAsync(SolutionName);
        var result = await OrderPages.PlannedDeliveryDates.RunAccessibilityScanAsync();

        AccessibilityReporter.Report(result, Output, "Review planned delivery dates page");

        var blocking = result.Violations
            .Where(v => v.Impact == "critical" || v.Impact == "serious")
            .ToList();

        Assert.True(
            blocking.Count == 0,
            $"Login page has {blocking.Count} critical or serious accessibility violations. See test output for detail.");
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task ConfirmQuantitiesPage_AccessibilityScan()
    {
        await OrderPages.GoToConfirmQuantitiesPageAsync(SolutionName);
        var result = await OrderPages.Quantity.RunAccessibilityScanAsync();

        AccessibilityReporter.Report(result, Output, "Confirm quantities page");

        var blocking = result.Violations
            .Where(v => v.Impact == "critical" || v.Impact == "serious")
            .ToList();

        Assert.True(
            blocking.Count == 0,
            $"Confirm quantities page has {blocking.Count} critical or serious accessibility violations. See test output for detail.");
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task OrderTypePage_RadioGroupHasLegend()
    {
        await OrderPages.LoginAsync();
        await OrderPages.GoToOrderTypePageAsync();
        await OrderPages.OrderType.AssertRadioGroupHasLegendAsync();
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task ServiceRecipientsOptionPage_RadioGroupHasLegend()
    {
        await OrderPages.GoToServiceRecipientsOptionPageAsync();
        await OrderPages.ServiceRecipients.AssertRadioGroupHasLegendAsync();
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task DeclarationPage_FieldsetHasLegend()
    {
        await OrderPages.GoToDeclarationPageAsync(SolutionName);
        await OrderPages.Declaration.AssertFieldsetHasLegendAsync();
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task CatalogueSolutionsPage_HeadingLevelsNotSkipped()
    {
        await OrderPages.GoToCatalogueSolutionsPageAsync();
        await OrderPages.CatalogueSolutions.AssertHeadingLevelsNotSkippedAsync();
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task CatalogueSolutionsPage_SearchLabelIsAssociated()
    {
        await OrderPages.GoToCatalogueSolutionsPageAsync();
        await OrderPages.CatalogueSolutions.AssertSearchLabelIsAssociatedAsync();
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task SolutionSummaryPage_BackToTopLinkRemoved()
    {
        await OrderPages.GoToSolutionSummaryPageAsync("AccuRx");
        await OrderPages.SolutionSummary.AssertBackToTopLinkNotPresentAsync();
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task QuantityOfCatalogueSolutionPage_HeadingLevelsNotSkipped()
    {
        await OrderPages.GoToQuantityOfCatalogueSolutionPageAsync(SolutionName);
        await OrderPages.Quantity.AssertHeadingLevelsNotSkippedAsync();
    }
}
