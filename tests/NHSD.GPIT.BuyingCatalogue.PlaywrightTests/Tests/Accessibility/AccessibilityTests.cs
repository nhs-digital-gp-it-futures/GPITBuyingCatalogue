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
    [Trait("Category", Categories.Accessibility)]
    public async Task DeclarationPage_HasPageTitle()
    {
        await orderPages.GoToDeclarationPageAsync(SolutionName);
        await orderPages.Declaration.AssertPageHasTitleAsync();
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task OrderCompletedPage_HasPageTitle()
    {
        await orderPages.GoToOrderCompletedPageAsync(SolutionName);
        await orderPages.ReviewOrder.AssertPageHasTitleAsync();
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

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task ReviewPlannedDeliveryDatesPage_AccessibilityScan()
    {
        await orderPages.GoToReviewPlannedDeliveryDatesPageAsync(SolutionName);
        var result = await orderPages.PlannedDeliveryDates.RunAccessibilityScanAsync();

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
        await orderPages.GoToConfirmQuantitiesPageAsync(SolutionName);
        var result = await orderPages.Quantity.RunAccessibilityScanAsync();

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
        await orderPages.LoginAsync();
        await orderPages.GoToOrderTypePageAsync();
        await orderPages.OrderType.AssertRadioGroupHasLegendAsync();
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task ServiceRecipientsOptionPage_RadioGroupHasLegend()
    {
        await orderPages.GoToServiceRecipientsOptionPageAsync();
        await orderPages.ServiceRecipients.AssertRadioGroupHasLegendAsync();
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task DeclarationPage_FieldsetHasLegend()
    {
        await orderPages.GoToDeclarationPageAsync(SolutionName);
        await orderPages.Declaration.AssertFieldsetHasLegendAsync();
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task CatalogueSolutionsPage_HeadingLevelsNotSkipped()
    {
        await orderPages.GoToCatalogueSolutionsPageAsync();
        await orderPages.CatalogueSolutions.AssertHeadingLevelsNotSkippedAsync();
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task CatalogueSolutionsPage_SearchLabelIsAssociated()
    {
        await orderPages.GoToCatalogueSolutionsPageAsync();
        await orderPages.CatalogueSolutions.AssertSearchLabelIsAssociatedAsync();
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task SolutionSummaryPage_BackToTopLinkRemoved()
    {
        await orderPages.GoToSolutionSummaryPageAsync("AccuRx");
        await orderPages.SolutionSummary.AssertBackToTopLinkNotPresentAsync();
    }

    [Fact]
    [Trait("Category", Categories.Accessibility)]
    public async Task QuantityOfCatalogueSolutionPage_HeadingLevelsNotSkipped()
    {
        await orderPages.GoToQuantityOfCatalogueSolutionPageAsync(SolutionName);
        await orderPages.Quantity.AssertHeadingLevelsNotSkippedAsync();
    }
}
