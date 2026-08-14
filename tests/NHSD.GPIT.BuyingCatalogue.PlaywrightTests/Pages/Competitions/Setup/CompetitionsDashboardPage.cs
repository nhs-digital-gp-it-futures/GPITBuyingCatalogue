using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Competitions.Setup;

public class CompetitionsDashboardPage : BasePage
{
    private ILocator CompetitionsLink => Page.GetByRole(AriaRole.Link, new() { Name = "Competitions", Exact = true });
    private ILocator CreateNewCompetitionLink => Page.GetByRole(AriaRole.Link, new() { Name = "Create new competition" });

    public CompetitionsDashboardPage(IPage page) : base(page) { }

    public async Task GoToCompetitionsAsync()
    {
        await CompetitionsLink.ClickAsync();
        await AssertHeadingAsync("Your organisation's competitions");
    }

    public async Task StartNewCompetitionAsync() => await CreateNewCompetitionLink.ClickAsync();
}
