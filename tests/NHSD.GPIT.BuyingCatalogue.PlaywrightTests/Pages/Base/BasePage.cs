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

    // Asserts the page has a meaningful title, not empty and not a generic fallback.
    public async Task AssertPageHasMeaningfulTitleAsync(string genericFallback = "Buying Catalogue")
    {
        var title = await Page.TitleAsync();

        Assert.False(string.IsNullOrWhiteSpace(title),
            "Page title is empty (WAVE: missing or uninformative page title, WCAG 2.4.2).");

        Assert.False(
            string.Equals(title.Trim(), genericFallback, System.StringComparison.OrdinalIgnoreCase),
            $"Page title is uninformative, just '{genericFallback}' (WAVE: uninformative page title, WCAG 2.4.2).");
    }
}
