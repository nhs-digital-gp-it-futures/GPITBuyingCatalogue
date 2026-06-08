using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.OrderType;

public class ServiceCategoryPage : BasePage
{
    public ServiceCategoryPage(IPage page) : base(page) { }

    public async Task SelectServiceCategoryAsync(string categoryName)
    {
        await Page.GetByRole(AriaRole.Radio, new() { Name = categoryName }).CheckAsync();
        await ClickSaveAndContinueAsync();
    }
}
