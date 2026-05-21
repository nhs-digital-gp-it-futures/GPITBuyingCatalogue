using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepTwo;

public class SolutionsAndServicesPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Solutions and services" });
    private ILocator StartLink => Page.GetByRole(AriaRole.Link, new() { Name = "Start" });

    public SolutionsAndServicesPage(IPage page) : base(page) { }

    public async Task NavigateAsync() =>
        await NavigationLink.ClickAsync();

    public async Task SelectCatalogueSolutionAsync(string solutionName)
    {
        await AssertHeadingAsync("Catalogue solutions");
        await Page.GetByRole(AriaRole.Radio, new() { Name = solutionName }).CheckAsync();
        await ClickSaveAndContinueAsync();
    }

    public async Task SelectPriceAsync()
    {
        await AssertHeadingAsync("Catalogue solution and services");
        await StartLink.ClickAsync();
        await AssertHeadingAsync("Price of Catalogue solution");
        await ClickSaveAndContinueAsync();
    }

    // Associated Service "Something Else"
    public async Task SelectAssociatedServiceWithVariantAsync(string serviceName, string serviceVariant)
    {
        await AssertHeadingAsync("Which catalogue solution does the service help implement?");
        await Page.GetByRole(AriaRole.Radio, new() { Name = serviceName }).CheckAsync();
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Add associated services");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = serviceVariant }).CheckAsync();
        await ClickSaveAndContinueAsync();
    }

    // Associated Service "Merger" (radio only)
    public async Task SelectAssociatedServiceAsync(string serviceName)
    {
        await AssertHeadingAsync("Which catalogue solution does the service help implement?");
        await Page.GetByRole(AriaRole.Radio, new() { Name = serviceName }).CheckAsync();
        await ClickSaveAndContinueAsync();
    }

    // Associated Service pricing
    public async Task SelectAssociatedServicePriceAsync()
    {
        await StartLink.ClickAsync();
        await AssertHeadingAsync("Price of Associated service");
        await ClickSaveAndContinueAsync();
    }

    // Continue past the edit page (Merger only)
    public async Task ContinuePastEditAsync()
    {
        await AssertHeadingAsync("Edit associated service");
        await ClickSaveAndContinueLinkAsync();
    }
}
