using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;
using static Microsoft.Playwright.Assertions;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepThree;

public class ImplementationMilestonesPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Implementation milestones and payment triggers" });
    private ILocator AddBespokeLink => Page.GetByRole(AriaRole.Link, new() { Name = "Add a bespoke milestone" });
    private ILocator MilestoneName => Page.GetByRole(AriaRole.Textbox, new() { Name = "Milestone name" });
    private ILocator PaymentTrigger => Page.GetByRole(AriaRole.Textbox, new() { Name = "Milestone payment trigger" });

    public ImplementationMilestonesPage(IPage page) : base(page) { }

    public async Task NavigateAndContinueAsync(string bespokeMilestoneName = "", string bespokePaymentTrigger = "")
    {
        await NavigationLink.ClickAsync();
        await AssertHeadingAsync("Implementation milestones and payment triggers");
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Do you want to add a bespoke milestone?");

        if (string.IsNullOrWhiteSpace(bespokeMilestoneName))
        {
            await Page.GetByRole(AriaRole.Radio, new() { Name = "No" }).CheckAsync();
            await ClickSaveAndContinueAsync();
            return;
        }

        await Page.GetByRole(AriaRole.Radio, new() { Name = "Yes" }).CheckAsync();
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Bespoke implementation milestone");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Milestone name" }).FillAsync(bespokeMilestoneName);
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Milestone payment trigger" }).FillAsync(bespokePaymentTrigger);
        await ClickSaveAndContinueAsync();

        await AssertHeadingAsync("Implementation milestones and payment triggers");
        await Expect(Page.GetByText(bespokeMilestoneName)).ToBeVisibleAsync();
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
