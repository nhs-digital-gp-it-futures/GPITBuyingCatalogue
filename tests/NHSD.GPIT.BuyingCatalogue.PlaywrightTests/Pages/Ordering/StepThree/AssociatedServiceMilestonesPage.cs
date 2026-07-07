using System.Threading.Tasks;
using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepThree;

public class AssociatedServiceMilestonesPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Associated service milestones" });
    private ILocator AddMilestoneLink => Page.GetByRole(AriaRole.Link, new() { Name = "Add a milestone" });
    private ILocator AssociatedServiceDropdown => Page.GetByLabel("Associated service name");
    private ILocator MilestoneName => Page.GetByRole(AriaRole.Textbox, new() { Name = "Milestone name" });
    private ILocator PaymentTrigger => Page.GetByRole(AriaRole.Textbox, new() { Name = "Milestone payment trigger" });

    public AssociatedServiceMilestonesPage(IPage page) : base(page) { }

    public async Task NavigateAndContinueAsync()
    {
        await NavigationLink.ClickAsync();
        await AssertHeadingAsync("Associated service milestones and payment triggers");
        await ClickSaveAndContinueAsync();
    }

    public async Task NavigateAndAddBespokeMilestoneAsync(string associatedService, string milestoneName, string paymentTrigger)
    {
        await NavigationLink.ClickAsync();
        await AssertHeadingAsync("Associated service milestones and payment triggers");

        await AddMilestoneLink.ClickAsync();
        await AssociatedServiceDropdown.SelectOptionAsync(new[] { associatedService });
        await MilestoneName.FillAsync(milestoneName);
        await PaymentTrigger.FillAsync(paymentTrigger);
        await ClickSaveAndContinueAsync();

        await ClickSaveAndContinueAsync();
    }
}
