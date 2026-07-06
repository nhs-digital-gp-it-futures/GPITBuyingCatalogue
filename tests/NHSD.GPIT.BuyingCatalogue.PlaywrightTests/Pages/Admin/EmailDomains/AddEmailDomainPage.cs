using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.EmailDomains;

public class AddEmailDomainPage : BasePage
{
    private ILocator EmailDomainInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Email domain" });

    public AddEmailDomainPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Add an email domain");

    public async Task AddEmailDomainAsync(string domain)
    {
        await EmailDomainInput.FillAsync(domain);
        await ClickSaveAndContinueAsync();
    }
}
