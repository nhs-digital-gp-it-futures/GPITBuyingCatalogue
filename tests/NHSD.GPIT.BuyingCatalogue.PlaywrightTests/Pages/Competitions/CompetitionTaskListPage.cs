using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;
using static Microsoft.Playwright.Assertions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions;

public class CompetitionTaskListPage : BasePage
{
    private const string TaskListHint = "Complete the following steps to carry out a competition.";
    private ILocator HintText => Page.Locator("h1 + div.nhsuk-hint");

    public CompetitionTaskListPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() =>
        await Expect(HintText).ToHaveTextAsync(TaskListHint);

    public async Task GoToServiceRecipientsAsync() =>
        await Page.GetByRole(AriaRole.Link, new() { Name = "service recipients" }).ClickAsync();

    public async Task GoToContractLengthAsync() =>
        await Page.GetByRole(AriaRole.Link, new() { Name = "Contract length" }).ClickAsync();

    public async Task GoToAwardCriteriaAsync() =>
        await Page.GetByRole(AriaRole.Link, new() { Name = "Award criteria" }).ClickAsync();

    public async Task GoToCalculatePriceAsync() =>
        await Page.GetByRole(AriaRole.Link, new() { Name = "Calculate price" }).ClickAsync();

    public async Task GoToViewResultsAsync() =>
        await Page.GetByRole(AriaRole.Link, new() { Name = "View results" }).ClickAsync();
}
