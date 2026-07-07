using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Users;

public class ManageUsersPage : BasePage
{
    private ILocator AddNewUserLink => Page.GetByRole(AriaRole.Link, new() { Name = "Add a new user" });
    private ILocator SearchInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Search" });
    private ILocator SearchButton => Page.GetByRole(AriaRole.Button, new() { Name = "Search" });

    public ManageUsersPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Manage users");

    public async Task GoToAddNewUserAsync() => await AddNewUserLink.ClickAsync();

    public async Task AssertUserExistsAsync(string email)
    {
        if (await SearchInput.IsVisibleAsync())
        {
            await SearchInput.FillAsync(email);
            await SearchButton.ClickAsync();
        }

        var userRow = Page.GetByRole(AriaRole.Row).Filter(new() { HasText = email });
        await userRow.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 5000 });
    }
}
