using System.Text.RegularExpressions;
using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.Dashboard;

public class OrderingDashboardPage : BasePage
{
    public OrderingDashboardPage(IPage page) : base(page) { }

    public async Task AssertOnDashboardAsync() =>
        await AssertHeadingAsync("Your organisation’s dashboard");

    public async Task GoToOrdersAsync()
    {        
        await Page.Locator("li.nhsuk-card-group__item").Filter(new() { HasText = "orders" })
                      .GetByRole(AriaRole.Link, new() { Name = "View orders" })
                      .ClickAsync();
        await AssertHeadingAsync("Your organisation’s orders");
    }

    public async Task CreateNewOrderAsync()
    {
        await Page.GetByRole(AriaRole.Link, new() { Name = "Create new order" }).ClickAsync();
    }
}
