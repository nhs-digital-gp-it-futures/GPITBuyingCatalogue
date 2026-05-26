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
}
