using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Setup;

public class CreateCompetitionPage : BasePage
{
    private ILocator CreateCompetitionLink => Page.GetByRole(AriaRole.Link, new() { Name = "Create competition" });
    private ILocator NameInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Competition name" });
    private ILocator DescriptionInput => Page.GetByRole(AriaRole.Textbox, new() { Name = "Competition description" });

    public CreateCompetitionPage(IPage page) : base(page) { }

    public async Task StartCreateAsync() => await CreateCompetitionLink.ClickAsync();

    public async Task EnterNameAndContinueAsync(string name, string description)
    {
        await AssertHeadingAsync("Create a competition");
        await NameInput.FillAsync(name);
        await DescriptionInput.FillAsync(description);
        await ClickSaveAndContinueAsync();
    }
}
