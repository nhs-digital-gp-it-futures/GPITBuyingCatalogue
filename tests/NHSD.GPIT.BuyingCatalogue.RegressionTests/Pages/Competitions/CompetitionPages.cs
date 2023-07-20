using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.Dashboard;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.SelectFilterType;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions
{
    public class CompetitionPages
    {
        public CompetitionPages(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
        {
            CompetitionDashboard = new CompetitionDashboard(driver, commonActions);
            SelectFilter = new SelectFilter(driver, commonActions);
            BeforeYouStart = new BeforeYouStart(driver, commonActions);
            ReviewFilter = new ReviewFilter(driver, commonActions);
            StartCompetition = new StartCompetition(driver, commonActions);
            SelectSolutions = new SelectSolutions(driver, commonActions);
            SolutionNotShortlisted = new SolutionNotShortlisted(driver, commonActions);
            Factory = factory;
            Driver = driver;
        }

        internal LocalWebApplicationFactory Factory { get; private set; }

        internal CompetitionDashboard CompetitionDashboard { get; }

        internal SelectFilter SelectFilter { get; }

        internal BeforeYouStart BeforeYouStart { get; }

        internal ReviewFilter ReviewFilter { get; }

        internal StartCompetition StartCompetition { get; }

        internal SelectSolutions SelectSolutions { get; }

        internal SolutionNotShortlisted SolutionNotShortlisted { get; }

        internal IWebDriver Driver { get; }
    }
}
