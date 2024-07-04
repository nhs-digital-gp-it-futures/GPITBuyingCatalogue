using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Competitions.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition.SelectFilterType
{
    public class ReviewFilter : PageBase
    {
        public ReviewFilter(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void ReviewYourFilterNoSolution()
        {
            CommonActions.PageLoadedCorrectGetIndex(
               typeof(CompetitionsDashboardController),
               nameof(CompetitionsDashboardController.ReviewFilter))
           .Should()
           .BeTrue();
        }

        public void ReviewYourFilterSingleSolution()
        {
            CommonActions.PageLoadedCorrectGetIndex(
               typeof(CompetitionsDashboardController),
               nameof(CompetitionsDashboardController.ReviewFilter))
           .Should()
           .BeTrue();

            CommonActions.ClickCreateCompitition();
        }

        public void ReviewYourFilterMultipleSolutions()
        {
            CommonActions.PageLoadedCorrectGetIndex(
               typeof(CompetitionsDashboardController),
               nameof(CompetitionsDashboardController.ReviewFilter))
           .Should()
           .BeTrue();

            CommonActions.ClickFirstCreateCompitition();
        }
    }
}
