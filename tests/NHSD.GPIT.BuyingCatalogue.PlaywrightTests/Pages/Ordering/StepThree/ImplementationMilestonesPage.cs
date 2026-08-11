using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepThree;

public class ImplementationMilestonesPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Implementation milestones and payment triggers" });
    private ILocator AddBespokeLink => Page.GetByRole(AriaRole.Link, new() { Name = "Add a bespoke milestone" });
    private ILocator MilestoneName => Page.GetByRole(AriaRole.Textbox, new() { Name = "Milestone name" });
    private ILocator PaymentTrigger => Page.GetByRole(AriaRole.Textbox, new() { Name = "Milestone payment trigger" });

    public ImplementationMilestonesPage(IPage page) : base(page) { }

    public async Task NavigateAndContinueAsync()
    {
        await NavigationLink.ClickAsync();
        await AssertHeadingAsync("Implementation milestones and payment triggers");
        await ClickSaveAndContinueAsync();
    }

    public async Task NavigateAndAddBespokeMilestoneAsync(string milestoneName, string paymentTrigger)
    {
        await NavigationLink.ClickAsync();
        await AssertHeadingAsync("Implementation milestones and payment triggers");
        await AssertHeadingLevelsNotSkippedAsync();

        await AddBespokeLink.ClickAsync();
        await MilestoneName.FillAsync(milestoneName);
        await PaymentTrigger.FillAsync(paymentTrigger);
        await ClickSaveAndContinueAsync();

        await ClickSaveAndContinueAsync();
    }
}
