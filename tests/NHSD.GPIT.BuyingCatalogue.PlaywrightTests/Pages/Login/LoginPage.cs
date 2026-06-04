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

    // reCAPTCHA renders inside an iframe hence use FrameLocator to reach it
    private IFrameLocator RecaptchaFrame => Page.FrameLocator("iframe[title='reCAPTCHA']");
    private ILocator RecaptchaCheckbox => RecaptchaFrame.Locator("#recaptcha-anchor");

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
        await CompleteRecaptchaAsync();
        await LoginButton.ClickAsync();
    }

    public async Task AssertLoginSuccessfulAsync() =>
        await AssertHeadingAsync("Your organisation's dashboard");

    private async Task CompleteRecaptchaAsync()
    {
        await RecaptchaCheckbox.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        await RecaptchaCheckbox.ClickAsync();

        // Wait for the checkbox to be checked before continuing
        await Page.FrameLocator("iframe[title='reCAPTCHA']")
            .Locator("#recaptcha-anchor[aria-checked='true']")
            .WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
    }
}
