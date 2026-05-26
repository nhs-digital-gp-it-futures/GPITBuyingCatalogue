using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepOne;

public class TimescalesPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Timescales for call-off agreement" });
    private ILocator DayInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Day" });
    private ILocator MonthInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Month", Exact = true });
    private ILocator YearInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Year" });
    private ILocator InitialPeriodInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "What is the call-off agreement initial period (in months)?" });
    private ILocator DurationInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "What is the call-off agreement duration (in months)?" });

    public TimescalesPage(IPage page) : base(page) { }

    public async Task NavigateAsync() =>
        await NavigationLink.ClickAsync();

    public async Task EnterTimescalesAsync(string day, string month, string year, string initialPeriod, string duration)
    {
        await AssertHeadingAsync("Timescales for call-off agreement");
        await DayInput.FillAsync(day);
        await MonthInput.FillAsync(month);
        await YearInput.FillAsync(year);
        await InitialPeriodInput.FillAsync(initialPeriod);
        await DurationInput.FillAsync(duration);
        await ClickSaveAndContinueAsync();
    }
}
