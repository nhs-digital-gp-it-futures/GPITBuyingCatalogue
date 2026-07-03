using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.EmailDomains;

public class DeleteEmailDomainPage : BasePage
{
    private ILocator DeleteButton => Page.GetByRole(AriaRole.Button, new() { Name = "Delete email domain" });

    public DeleteEmailDomainPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Delete this email domain?");

    public async Task ConfirmDeleteAsync() => await DeleteButton.ClickAsync();
}
