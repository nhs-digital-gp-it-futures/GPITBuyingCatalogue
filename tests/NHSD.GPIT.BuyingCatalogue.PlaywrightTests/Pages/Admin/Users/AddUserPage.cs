using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Users;

public class AddUserPage : BasePage
{
    private ILocator OrganisationInput => Page.Locator("input[role='combobox']:visible");
    private ILocator SuggestionsList => Page.Locator("ul[role='listbox']:visible");
    private ILocator FirstNameInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "First name" });
    private ILocator LastNameInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Last name" });
    private ILocator EmailInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Email address" });

    public AddUserPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Add user");

    public async Task AddUserAsync(
        string organisation,
        string firstName,
        string lastName,
        string email,
        string accountType,
        string accountStatus)
    {
        await SelectOrganisationAsync(organisation);
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await EmailInput.FillAsync(email);
        await Page.GetByRole(AriaRole.Radio, new() { Name = accountType }).CheckAsync();
        await Page.GetByRole(AriaRole.Radio, new() { Name = accountStatus, Exact = true }).CheckAsync();
        await ClickSaveAndContinueAsync();
    }

    private async Task SelectOrganisationAsync(string organisationName)
    {
        await OrganisationInput.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await OrganisationInput.ClickAsync();
        await OrganisationInput.FillAsync(string.Empty);
        await OrganisationInput.PressSequentiallyAsync(organisationName);

        await SuggestionsList.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await SuggestionsList.GetByText(organisationName, new() { Exact = true }).ClickAsync();
    }
}
