using System.Text.RegularExpressions;
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
}
