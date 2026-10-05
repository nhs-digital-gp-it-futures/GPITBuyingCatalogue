using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Setup;

public class ImplementationRequirementsPage : BasePage
{
    private ILocator RequirementInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "What implementation" });

    public ImplementationRequirementsPage(IPage page) : base(page) { }

    public async Task AddAsync(string requirement)
    {
        await AssertHeadingAsync("Implementation requirements");
        await RequirementInput.FillAsync(requirement);
        await ClickSaveAndContinueAsync();
    }
}
