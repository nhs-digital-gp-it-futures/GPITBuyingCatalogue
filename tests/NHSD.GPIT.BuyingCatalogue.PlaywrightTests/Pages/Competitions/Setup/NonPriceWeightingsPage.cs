using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Setup;

public class NonPriceWeightingsPage : BasePage
{
    private ILocator FeaturesInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Features weighting as a" });
    private ILocator ImplementationInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Implementation weighting as a" });
    private ILocator InteroperabilityInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Interoperability weighting as" });
    private ILocator ServiceLevelInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Service level weighting as a" });

    public NonPriceWeightingsPage(IPage page) : base(page) { }

    public async Task EnterWeightingsAndContinueAsync(
        string features, string implementation, string interoperability, string serviceLevel)
    {
        await AssertHeadingAsync("How would you like to weight your non-price elements for this competition?");
        await FeaturesInput.FillAsync(features);
        await ImplementationInput.FillAsync(implementation);
        await InteroperabilityInput.FillAsync(interoperability);
        await ServiceLevelInput.FillAsync(serviceLevel);
        await ClickSaveAndContinueAsync();
    }
}
