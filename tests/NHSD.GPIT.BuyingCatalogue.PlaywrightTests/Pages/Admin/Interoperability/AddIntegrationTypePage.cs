using System.Threading.Tasks;
using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Admin.Interoperability;

public class AddIntegrationTypePage : BasePage
{
    private ILocator NameInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Integration type name" });
    private ILocator DescriptionInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Integration type description" });

    public AddIntegrationTypePage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() => await AssertHeadingAsync("Add integration type");

    public async Task AddAsync(string name, string description = "")
    {
        await NameInput.FillAsync(name);

        if (!string.IsNullOrWhiteSpace(description))
            await DescriptionInput.FillAsync(description);

        await ClickSaveAndContinueAsync();
    }
}
