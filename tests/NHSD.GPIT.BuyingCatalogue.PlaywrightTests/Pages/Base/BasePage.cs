using System.Text.RegularExpressions;
using Deque.AxeCore.Commons;
using Deque.AxeCore.Playwright;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

public abstract class BasePage
{
    protected readonly IPage Page;

    protected BasePage(IPage page)
    {
        Page = page;
    }

    protected async Task ClickSaveAndContinueAsync() =>
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save and continue" }).ClickAsync();

    protected async Task ClickSaveAndContinueLinkAsync() =>
        await Page.GetByRole(AriaRole.Link, new() { Name = "Save and continue" }).ClickAsync();

    protected async Task ClickContinueLinkAsync() =>
        await Page.GetByRole(AriaRole.Link, new() { Name = "Continue" }).ClickAsync();

    protected async Task AssertHeadingAsync(string heading) =>
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = heading })).ToBeVisibleAsync();

    protected async Task AssertUrlContainsAsync(string fragment) =>
        await Expect(Page).ToHaveURLAsync(new Regex(fragment));

    // Runs axe-core against the current page, restricted to the WCAG rule sets.
    // Returns the full result object for the caller to inspect or format.
    public async Task<AxeResult> RunAccessibilityScanAsync()
    {
        var options = new AxeRunOptions
        {
            RunOnly = new RunOnlyOptions
            {
                Type = "tag",
                Values = new List<string>
            {
                "wcag2a",
                "wcag2aa",
                "wcag21a",
                "wcag21aa",
                "wcag22aa"
            }
            }
        };

        return await Page.RunAxe(options);
    }

    public async Task AssertPageHasTitleAsync(string genericFallback = "Buying Catalogue")
    {
        var title = await Page.TitleAsync();

        Assert.False(string.IsNullOrWhiteSpace(title),
            "Accessibility: Page title is missing. " +
            "Pages must have a title that describes their topic or purpose (WCAG 2.4.2).");
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
                    $"Accessibility: Heading hierarchy skips from " +
                    $"h{previousLevel} to h{currentLevel}. " +
                    "Review the heading structure to ensure relationships are " +
                    "programmatically represented correctly " +
                    "(WAVE skipped heading alert; related to WCAG 1.3.1).");
            }

            previousLevel = currentLevel;
        }
    }
}
