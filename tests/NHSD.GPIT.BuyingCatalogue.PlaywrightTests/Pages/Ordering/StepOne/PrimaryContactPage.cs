using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepOne;

public class PrimaryContactPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Primary contact details" });
    private ILocator FirstNameInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "First name" });
    private ILocator LastNameInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Last name" });
    private ILocator PhoneInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Telephone number" });
    private ILocator EmailInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Email address" });

    public PrimaryContactPage(IPage page) : base(page) { }

    public async Task NavigateAsync() =>
        await NavigationLink.ClickAsync();

    public async Task EnterContactDetailsAsync(string firstName, string lastName, string phone, string email)
    {
        await FirstNameInput.FillAsync(firstName);
        await LastNameInput.FillAsync(lastName);
        await PhoneInput.FillAsync(phone);
        await EmailInput.FillAsync(email);
        await ClickSaveAndContinueAsync();
    }
}
