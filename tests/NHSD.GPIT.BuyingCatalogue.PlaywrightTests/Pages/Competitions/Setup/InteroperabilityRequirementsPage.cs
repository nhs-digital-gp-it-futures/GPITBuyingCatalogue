using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Setup;

public class InteroperabilityRequirementsPage : BasePage
{
    public InteroperabilityRequirementsPage(IPage page) : base(page) { }

    public async Task AddAsync(string[] options)
    {
        await AssertHeadingAsync("Interoperability requirements");

        foreach (var option in options)
            await Page.GetByRole(AriaRole.Checkbox, new() { Name = option }).CheckAsync();

        await ClickSaveAndContinueAsync();
    }
}
