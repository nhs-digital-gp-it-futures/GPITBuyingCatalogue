using Microsoft.Playwright;
using NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.Base;

namespace NHSD.GPIT.BuyingCatalogue.PlaywrightTests.Pages.CatalogueSolutions;

public class SolutionSummaryPage : BasePage
{
    public SolutionSummaryPage(IPage page) : base(page) { }

    public async Task AssertOnPageAsync(string solutionName)
    {
        await AssertHeadingAsync(solutionName);
    }

    public async Task AssertBackToTopLinkNotPresentAsync()
    {
        var backToTopLink = Page.GetByRole(AriaRole.Link, new() { Name = "Back to top" });
        var linkCount = await backToTopLink.CountAsync();

        Assert.True(linkCount == 0,
            "Back to top link is still present on the page, but it has been removed as a design decision.");
    }
}
