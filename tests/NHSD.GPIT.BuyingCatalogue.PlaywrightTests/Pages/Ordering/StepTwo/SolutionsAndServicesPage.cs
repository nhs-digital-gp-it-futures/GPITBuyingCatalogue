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
}
