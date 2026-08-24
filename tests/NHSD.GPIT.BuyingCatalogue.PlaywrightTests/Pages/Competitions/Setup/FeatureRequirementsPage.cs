using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Setup;

public class FeatureRequirementsPage : BasePage
{
    private ILocator RequirementInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Feature requirement" });

    public FeatureRequirementsPage(IPage page) : base(page) { }

    public async Task AddAsync(string requirementType, string requirement)
    {
        await AssertHeadingAsync("Features requirements");
        await Page.GetByRole(AriaRole.Radio, new() { Name = requirementType }).CheckAsync();
        await RequirementInput.FillAsync(requirement);
        await ClickSaveAndContinueAsync();
    }
}
