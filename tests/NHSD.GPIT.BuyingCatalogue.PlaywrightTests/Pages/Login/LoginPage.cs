using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Login;

public class LoginPage : BasePage
{
    private ILocator LoginLink => Page.GetByRole(AriaRole.Link, new() { Name = "Log in" });
    private ILocator EmailInput => Page.GetByLabel("Email");
    private ILocator PasswordInput => Page.GetByLabel("Password");
    private ILocator LoginButton => Page.GetByRole(AriaRole.Button, new() { Name = "Log in" });

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

    public async Task AssertLoginSuccessfulAsync(string expectedHeading = "Your organisation's dashboard") =>
        await AssertHeadingAsync(expectedHeading);

    private async Task CompleteRecaptchaAsync()
    {
        await RecaptchaCheckbox.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var checkedState = Page.FrameLocator("iframe[title='reCAPTCHA']")
            .Locator("#recaptcha-anchor[aria-checked='true']");

        for (var attempt = 1; attempt <= 2; attempt++)
        {
            await RecaptchaCheckbox.ClickAsync();

            try
            {
                await checkedState.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 10000 });
                return;
            }
            catch (TimeoutException)
            {
                if (attempt == 2) throw;
            }
        }
    }
}
