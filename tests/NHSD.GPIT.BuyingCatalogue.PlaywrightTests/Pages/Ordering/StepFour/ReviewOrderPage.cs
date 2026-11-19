using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepFour;

public class ReviewOrderPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Review and complete order" });
    private ILocator CompleteOrderButton => Page.GetByRole(AriaRole.Button, new() { Name = "Complete order" });

    public ReviewOrderPage(IPage page) : base(page) { }

    public async Task NavigateAsync()
    {
        await NavigationLink.ClickAsync();
        await AssertHeadingAsync("Review and complete order");
    }

    public async Task CompleteOrderAsync(string completedHeading = "Order completed")
    {
        await CompleteOrderButton.ClickAsync();
        await AssertHeadingAsync(completedHeading);
    }

    public async Task AssertOrderCompletedAsync() =>
    await AssertHeadingAsync("Order completed");
}
