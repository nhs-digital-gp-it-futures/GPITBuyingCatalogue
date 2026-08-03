using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Solutions;

public class ManageSolutionsPage : BasePage
{
    private ILocator AddSolutionLink => Page.GetByRole(AriaRole.Link, new() { Name = "Add a solution" });

    public ManageSolutionsPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Manage catalogue solutions");

    public async Task GoToAddSolutionAsync() => await AddSolutionLink.ClickAsync();

    public async Task AssertSolutionExistsAsync(string solutionName)
    {
        var row = Page.GetByRole(AriaRole.Row).Filter(new() { HasText = solutionName });
        await row.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
    }
}
