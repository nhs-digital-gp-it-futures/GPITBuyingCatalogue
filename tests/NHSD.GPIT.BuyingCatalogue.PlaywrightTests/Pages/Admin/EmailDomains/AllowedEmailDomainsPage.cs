using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.EmailDomains;

public class AllowedEmailDomainsPage : BasePage
{
    private ILocator AddEmailDomainLink => Page.GetByRole(AriaRole.Link, new() { Name = "Add an email domain" });

    public AllowedEmailDomainsPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Allowed email domains");

    public async Task GoToAddEmailDomainAsync() => await AddEmailDomainLink.ClickAsync();

    public async Task DeleteEmailDomainAsync(string domain)
    {
        await Page.GetByRole(AriaRole.Row)
            .Filter(new() { HasText = domain })
            .GetByRole(AriaRole.Link, new() { Name = "Delete" })
            .ClickAsync();
    }
}
