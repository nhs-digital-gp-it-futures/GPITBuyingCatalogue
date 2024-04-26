using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Competitions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.Dashboard
{
    public class CompetitionDashboard : PageBase
    {
        public CompetitionDashboard(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void CompetitionTriage()
        {
            CommonActions.PageLoadedCorrectGetIndex(
                typeof(BuyerDashboardController),
                nameof(BuyerDashboardController.Index))
                .Should().BeTrue();

            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.ViewCompetitions);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CompetitionsDashboardController),
                nameof(CompetitionsDashboardController.Index))
                .Should().BeTrue();

            CommonActions.ClickLinkElement(CompetitionsDashboardObjects.CreateCompetitionLink);
        }
    }
}
