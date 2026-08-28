using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Setup;

public class ServiceLevelRequirementsPage : BasePage
{
    private static readonly string[] Weekdays = { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };

    private ILocator FromInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "From" });
    private ILocator UntilInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Until" });

    public ServiceLevelRequirementsPage(IPage page) : base(page) { }

    public async Task AddAsync(string from, string until)
    {
        await AssertHeadingAsync("Service level requirements");
        await FromInput.FillAsync(from);
        await UntilInput.FillAsync(until);

        foreach (var day in Weekdays)
            await Page.GetByRole(AriaRole.Checkbox, new() { Name = day }).CheckAsync();

        await Page.GetByRole(AriaRole.Radio, new() { Name = "Yes, include Bank Holidays" }).CheckAsync();
        await ClickSaveAndContinueAsync();
    }
}
