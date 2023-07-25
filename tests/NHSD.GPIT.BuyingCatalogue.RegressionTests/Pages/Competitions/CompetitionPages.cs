using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.Dashboard;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOne;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition.SelectFilterType;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition.SolutionSelection;
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
            CompetitionStepOne = new CompetitionStepOne(driver, commonActions);
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

        internal CompetitionStepOne CompetitionStepOne { get; }

        internal IWebDriver Driver { get; }

        public void StepOnePrepareCompetition(string filterType, string competitionName, int addNumberOfSolutions)
        {
            SelectFilter.SelectFilterForNewCompetition(filterType);

            ReviewFilter.ReviewYourFilter();

            StartCompetition.CreateCompetition(competitionName);

            int competitionId = CompetitionId();
            int filterId = GetFilterId(competitionId);

            if (filterId == 2)
            {
                SelectSolutions.AddSolutions(5);
                SolutionNotShortlisted.SolutionNotIncludedInShortlisting();
            }
        }

        private int CompetitionId()
        {
            string url = Driver.Url;
            int charPos = url.LastIndexOf("competitions/") + "competitions".Length + 1;
            int charLength = url.IndexOf("/select-solutions") - charPos;
            int competitionId = int.Parse(url.Substring(charPos, charLength));
            return competitionId;
        }

        private int GetFilterId(int competitionId)
        {
            using var dbContext = Factory.DbContext;

            var result = dbContext.Competitions
                .Where(x => x.Id == competitionId)
                .Select(y => y.FilterId).First();

            return result;
        }
    }
}
