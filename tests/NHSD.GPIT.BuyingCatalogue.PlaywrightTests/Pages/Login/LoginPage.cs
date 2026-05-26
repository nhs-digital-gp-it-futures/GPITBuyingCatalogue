using System.Threading.Tasks;
using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Login;

public class LoginPage : BasePage
{
    private ILocator LoginLink => Page.GetByRole(AriaRole.Link, new() { Name = "Log in" });
    private ILocator EmailInput => Page.GetByLabel("Email");
    private ILocator PasswordInput => Page.GetByLabel("Password");
    private ILocator LoginButton => Page.GetByRole(AriaRole.Button, new() { Name = "Log in" });

    public LoginPage(IPage page) : base(page) { }

    public async Task NavigateAsync(string baseUrl)
    {
        await Page.GotoAsync(baseUrl);
        await LoginLink.ClickAsync();
    }

    public async Task LoginAsync(string email, string password)
    {
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }

    public async Task AssertLoginSuccessfulAsync() =>
        await AssertHeadingAsync("Your organisation's dashboard");
}
