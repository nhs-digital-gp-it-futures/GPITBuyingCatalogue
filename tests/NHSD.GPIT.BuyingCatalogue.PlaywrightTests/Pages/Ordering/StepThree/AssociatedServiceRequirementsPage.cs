using System.Threading.Tasks;
using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Ordering.StepThree;

public class AssociatedServiceRequirementsPage : BasePage
{
    private ILocator NavigationLink => Page.GetByRole(AriaRole.Link, new() { Name = "Associated service requirements" });
    private ILocator AddRequirementLink => Page.GetByRole(AriaRole.Link, new() { Name = "Add a requirement" });
    private ILocator AssociatedServiceDropdown => Page.GetByLabel("Associated service name");
    private ILocator RequirementText => Page.GetByRole(AriaRole.Textbox, new() { Name = "What specific requirements do" });

    public AssociatedServiceRequirementsPage(IPage page) : base(page) { }

    public async Task NavigateAndContinueAsync()
    {
        await NavigationLink.ClickAsync();
        await AssertHeadingAsync("Associated service requirements");
        await ClickSaveAndContinueAsync();
    }

    public async Task NavigateAndAddRequirementAsync(string associatedService, string requirementText, bool requiresAdditionalDetails = false)
    {
        await NavigationLink.ClickAsync();
        await AssertHeadingAsync("Associated service requirements");

        await AddRequirementLink.ClickAsync();
        await AssociatedServiceDropdown.SelectOptionAsync(new[] { associatedService });
        await RequirementText.FillAsync(requirementText);

        var radioName = requiresAdditionalDetails ? "Yes" : "No";
        await Page.GetByRole(AriaRole.Radio, new() { Name = radioName }).CheckAsync();

        await ClickSaveAndContinueAsync();
        await ClickSaveAndContinueAsync();
    }
}
