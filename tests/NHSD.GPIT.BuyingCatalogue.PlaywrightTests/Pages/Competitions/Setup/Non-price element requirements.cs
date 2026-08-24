using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Setup;

public class NonPriceElementsPage : BasePage
{
    public NonPriceElementsPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync() =>
        await AssertHeadingAsync("Non-price elements");

    public async Task GoToAddFeatureRequirementsAsync() =>
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add feature requirements" }).ClickAsync();

    public async Task GoToAddImplementationRequirementsAsync() =>
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add implementation requirements" }).ClickAsync();

    public async Task GoToAddInteroperabilityRequirementsAsync() =>
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add interoperability requirements" }).ClickAsync();

    public async Task GoToAddServiceLevelRequirementsAsync() =>
        await Page.GetByRole(AriaRole.Link, new() { Name = "Add service level requirements" }).ClickAsync();

    public async Task SaveAndContinueAsync() =>
    await ClickSaveAndContinueLinkAsync();
}
