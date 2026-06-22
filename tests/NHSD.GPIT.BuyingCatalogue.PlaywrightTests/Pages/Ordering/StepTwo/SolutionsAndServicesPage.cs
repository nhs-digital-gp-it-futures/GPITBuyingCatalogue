using System.Threading.Tasks;
using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepTwo;

/// <summary>
/// Page object for step 2 of the ordering journey, covering catalogue solutions,
/// associated services, additional services, and pricing selection.
/// </summary>
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

    // Associated Service Only — "Something Else" (radio + variant checkbox)
    public async Task SelectAssociatedServiceWithVariantAsync(string serviceName, string serviceVariant)
    {
        await AssertHeadingAsync("Which catalogue solution does the service help implement?");
        await Page.GetByRole(AriaRole.Radio, new() { Name = serviceName }).CheckAsync();
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Add associated services");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = serviceVariant }).CheckAsync();
        await ClickSaveAndContinueAsync();
    }

    // Associated Service Only — "Merger" (radio only)
    public async Task SelectAssociatedServiceAsync(string serviceName)
    {
        await AssertHeadingAsync("Which catalogue solution does the service help implement?");
        await Page.GetByRole(AriaRole.Radio, new() { Name = serviceName }).CheckAsync();
        await ClickSaveAndContinueAsync();
    }

    // Associated Service Only — pricing
    public async Task SelectAssociatedServicePriceAsync()
    {
        await StartLink.ClickAsync();
        await AssertHeadingAsync("Price of Associated service");
        await ClickSaveAndContinueAsync();
    }

    // Associated Service Only — continue past edit page (Merger)
    public async Task ContinuePastEditAsync()
    {
        await AssertHeadingAsync("Edit associated service");
        await ClickSaveAndContinueLinkAsync();
    }

    // Add an associated OR additional service on top of a catalogue solution
    public async Task AddOnServiceToCatalogueAsync(AddOnServiceType addOn, string serviceName)
    {
        await AssertHeadingAsync("Edit solutions and services");
        await Page.GetByRole(AriaRole.Link, new() { Name = addOn.LinkText() }).ClickAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = serviceName }).CheckAsync();
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Catalogue solution and services");
        await StartLink.ClickAsync();
        await AssertHeadingAsync(addOn.PriceHeading());
        await ClickSaveAndContinueAsync();
    }

    // Remove an existing add-on service from the Edit page
    public async Task RemoveAddOnServiceAsync(AddOnServiceType addOn, string serviceName)
    {
        await AssertHeadingAsync("Edit solutions and services");
        await Page.GetByRole(AriaRole.Link, new() { Name = $"Remove {serviceName}" }).ClickAsync();

        var removeHeading = addOn == AddOnServiceType.Additional
            ? "Remove Additional service"
            : "Remove Associated service";
        await AssertHeadingAsync(removeHeading);

        await Page.GetByRole(AriaRole.Radio, new() { Name = "Yes, I confirm I want to" }).CheckAsync();
        await ClickSaveAndContinueAsync();
    }

    // Replace an existing add-on with a different one — removes old, adds new, prices it
    public async Task ReplaceAddOnServiceAsync(AddOnServiceType addOn, string oldServiceName, string newServiceName)
    {
        await RemoveAddOnServiceAsync(addOn, oldServiceName);

        // After removal we're back on the Edit page — add the replacement
        await Page.GetByRole(AriaRole.Link, new() { Name = addOn.LinkText() }).ClickAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = newServiceName }).CheckAsync();
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Catalogue solution and services");
        await StartLink.ClickAsync();
        await AssertHeadingAsync(addOn.PriceHeading());
        await ClickSaveAndContinueAsync();
    }

    // Change the catalogue solution from the Edit page — picks a new solution, confirms the change, re-prices
    public async Task ChangeCatalogueSolutionAsync(string newSolutionName)
    {
        await AssertHeadingAsync("Edit solutions and services");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Change Catalogue solution" }).ClickAsync();

        await AssertHeadingAsync("Catalogue solutions");
        await Page.GetByRole(AriaRole.Radio, new() { Name = newSolutionName }).CheckAsync();
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Are you sure you want to change your Catalogue solution?");
        await Page.GetByRole(AriaRole.Radio, new() { Name = "Yes, I want to confirm changes to my Catalogue solution" }).CheckAsync();
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Catalogue solution and services");
        await StartLink.ClickAsync();
        await AssertHeadingAsync("Price of Catalogue solution");
        await ClickSaveAndContinueAsync();
    }
}
