using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Setup;

public class AwardCriteriaWeightingsPage : BasePage
{
    private ILocator PriceWeightingInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Price weighting as a percentage", Exact = true });
    private ILocator NonPriceWeightingInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Non-price weighting as a" });

    public AwardCriteriaWeightingsPage(IPage page) : base(page) { }

    public async Task EnterWeightingsAndContinueAsync(string priceWeighting, string nonPriceWeighting)
    {
        await AssertHeadingAsync("How would you like to weight your award criteria for this competition?");
        await PriceWeightingInput.FillAsync(priceWeighting);
        await NonPriceWeightingInput.FillAsync(nonPriceWeighting);
        await ClickSaveAndContinueAsync();
    }
}
