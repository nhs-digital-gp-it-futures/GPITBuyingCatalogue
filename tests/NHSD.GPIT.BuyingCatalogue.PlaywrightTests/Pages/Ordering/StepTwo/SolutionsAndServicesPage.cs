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
    
    public async Task SelectAssociatedServiceSomeThingElseAsync(string serviceName, string serviceVariant)
    {
        await AssertHeadingAsync("Which catalogue solution does the service help implement?");
        await Page.GetByRole(AriaRole.Radio, new() { Name = serviceName }).CheckAsync();
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Add associated services");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = serviceVariant }).CheckAsync();
        await ClickSaveAndContinueAsync();
    }

    public async Task SelectAssociatedServiceMergerAsync(string serviceName)
    {
        await AssertHeadingAsync("Which catalogue solution does the service help implement?");
        await Page.GetByRole(AriaRole.Radio, new() { Name = serviceName }).CheckAsync();
        await ClickSaveAndContinueAsync();
    }

    public async Task SelectAssociatedServicePriceAsync()
    {
        await StartLink.ClickAsync();
        await AssertHeadingAsync("Price of Associated service");
        await ClickSaveAndContinueAsync();
    }

    public async Task ContinuePastEditAsync()
    {
        await AssertHeadingAsync("Edit associated service");
        await ClickSaveAndContinueLinkAsync();
    }

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
}
