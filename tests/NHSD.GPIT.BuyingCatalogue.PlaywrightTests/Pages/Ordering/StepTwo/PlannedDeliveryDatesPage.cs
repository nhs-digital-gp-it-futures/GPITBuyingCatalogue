using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepTwo;

public class PlannedDeliveryDatesPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Planned delivery dates" });
    private ILocator DayInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Day" });
    private ILocator MonthInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Month" });
    private ILocator YearInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Year" });
    private ILocator YesRadio => Page.GetByRole(AriaRole.Radio, new() { Name = "Yes" });

    public PlannedDeliveryDatesPage(IPage page) : base(page) { }

    public async Task NavigateAsync() =>
        await NavigationLink.ClickAsync();

    public async Task EnterDeliveryDateAsync(string day, string month, string year)
    {
        await AssertHeadingAsync("Planned delivery date");
        await DayInput.FillAsync(day);
        await MonthInput.FillAsync(month);
        await YearInput.FillAsync(year);
        await YesRadio.CheckAsync();
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Review planned delivery dates");
        await ClickSaveAndContinueLinkAsync();
    }

    public async Task EnterDeliveryDateAsync(string day, string month, string year, bool stopOnReview = false)
    {
        await AssertHeadingAsync("Planned delivery date");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Day" }).FillAsync(day);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Month" }).FillAsync(month);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Year" }).FillAsync(year);
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Yes" }).CheckAsync();
        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Review planned delivery dates");

        if (stopOnReview)
            return;

        await ClickSaveAndContinueLinkAsync();
    }

    // Planned delivery date alternate route
    public async Task EnterDeliveryDatePerRecipientAsync(
        string day, string month, string year,
        string[] practices, string perRecipientDay)
    {
        await AssertHeadingAsync("Planned delivery date");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Day" }).FillAsync(day);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Month" }).FillAsync(month);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Year" }).FillAsync(year);

        await Page.GetByRole(AriaRole.Radio, new() { Name = "No" }).CheckAsync();
        await ClickSaveAndContinueAsync();
        
        await AssertHeadingAsync("Which organisations do you want to change dates for?");
        foreach (var practice in practices)
            await Page.GetByRole(AriaRole.Checkbox, new() { Name = practice }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Continue" }).ClickAsync();

        await AssertHeadingAsync("Change delivery dates");
        for (var i = 0; i < practices.Length; i++)
        {
            await Page.GetByRole(AriaRole.Link, new() { Name = "Change" }).Nth(i).ClickAsync();
            await AssertHeadingAsync($"Change delivery dates for {practices[i]}");

            await Page.GetByLabel("Day").First.FillAsync(perRecipientDay);

            await ClickSaveAndContinueAsync();
            await AssertHeadingAsync("Change delivery dates");
        }

        await ClickSaveAndContinueAsync();
        await AssertHeadingAsync("Review planned delivery dates");
        await ClickSaveAndContinueLinkAsync();
    }
}
