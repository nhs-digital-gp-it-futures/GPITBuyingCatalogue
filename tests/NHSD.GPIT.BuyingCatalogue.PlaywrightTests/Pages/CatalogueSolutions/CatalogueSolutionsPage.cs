using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;
using static Microsoft.Playwright.Assertions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.CatalogueSolutions;

public class CatalogueSolutionsPage : BasePage
{
    public CatalogueSolutionsPage(IPage page) : base(page) { }

    public async Task NavigateAsync()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "Catalogue solutions" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Catalogue solutions", Level = 1 })).ToBeVisibleAsync();
    }

    public async Task AssertHeadingLevelsNotSkippedAsync()
    {
        var headings = await Page.Locator("h1, h2, h3, h4, h5, h6").AllAsync();

        var previousLevel = 0;

        foreach (var heading in headings)
        {
            var tagName = await heading.EvaluateAsync<string>("el => el.tagName");
            var currentLevel = int.Parse(tagName[1].ToString());

            if (previousLevel > 0)
            {
                Assert.True(
                    currentLevel <= previousLevel + 1,
                    $"Heading level skipped: h{previousLevel} is followed by h{currentLevel} " +
                    $"(WAVE: skipped heading level, WCAG 1.3.1).");
            }

            previousLevel = currentLevel;
        }
    }

    public async Task AssertSearchLabelIsAssociatedAsync()
    {
        var label = Page.GetByText("Search by supplier or solution name", new() { Exact = true });
        var labelCount = await label.CountAsync();

        Assert.True(labelCount > 0,
            "Search label 'Search by supplier or solution name' was not found on the page.");

        var forAttribute = await label.First.GetAttributeAsync("for");

        Assert.False(string.IsNullOrWhiteSpace(forAttribute),
            "Search label has no 'for' attribute, so it isn't associated with any control " +
            "(WAVE: orphaned form label, WCAG 1.3.1 / 4.1.2).");

        var associatedControl = Page.Locator($"#{forAttribute}");
        var controlCount = await associatedControl.CountAsync();

        Assert.True(controlCount > 0,
            $"Search label points to 'for=\"{forAttribute}\"' but no element with that id exists on the page " +
            "(WAVE: orphaned form label, WCAG 1.3.1 / 4.1.2).");
    }

    public async Task SelectSolutionAsync(string solutionName)
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = solutionName, Exact = true }).ClickAsync();
    }
}
