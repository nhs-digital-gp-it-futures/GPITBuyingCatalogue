using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Setup;

public class RefineShortlistPage : BasePage
{
    public RefineShortlistPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() =>
        await AssertHeadingAsync("Refine shortlist for this competition");

    public async Task SelectSolutionsAndContinueAsync(params string[] solutionNames)
    {
        foreach (var name in solutionNames)
            await SelectSolutionAsync(name);

        await ClickSaveAndContinueAsync();
    }

    private async Task SelectSolutionAsync(string solutionName)
    {
        var idAttribute = await Page
            .Locator($"input[value='{solutionName}'][name$='.SolutionName']")
            .GetAttributeAsync("id");

        if (idAttribute is null)
            throw new InvalidOperationException($"Could not find solution '{solutionName}' on the refine shortlist page.");

        var index = idAttribute.Split('_')[1];
        await Page.Locator($"#Solutions_{index}__Selected").CheckAsync();
    }
}
