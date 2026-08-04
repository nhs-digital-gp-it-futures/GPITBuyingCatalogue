using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepThree;

public class DeclarationPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Declaration" });
    private ILocator AgreeCheckbox => Page.GetByRole(AriaRole.Checkbox, new() { Name = "I understand and agree to the" });

    public DeclarationPage(IPage page) : base(page) { }

    public async Task NavigateAndAgreeAsync()
    {
        await NavigationLink.ClickAsync();
        await AssertHeadingAsync("Declaration");
        await AgreeCheckbox.CheckAsync();
        await ClickSaveAndContinueAsync();
    }

    public async Task NavigateAsync() =>
    await NavigationLink.ClickAsync();

    public async Task AssertOnPageAsync() =>
        await AssertHeadingAsync("Declaration");

    public async Task AssertFieldsetHasLegendAsync()
    {
        var fieldset = Page.Locator("fieldset:has(input[type='checkbox'])");
        var fieldsetCount = await fieldset.CountAsync();

        Assert.True(fieldsetCount > 0,
            "Declaration checkbox is not wrapped in a fieldset (WAVE: fieldset missing legend, WCAG 1.3.1).");

        var legend = fieldset.Locator("legend");
        var legendCount = await legend.CountAsync();

        Assert.True(legendCount > 0,
            "Declaration fieldset has no legend (WAVE: fieldset missing legend, WCAG 1.3.1).");

        var legendText = await legend.First.InnerTextAsync();
        Assert.False(string.IsNullOrWhiteSpace(legendText),
            "Declaration fieldset legend is empty (WAVE: fieldset missing legend, WCAG 1.3.1).");
    }
}
