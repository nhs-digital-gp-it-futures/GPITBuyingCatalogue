using Microsoft.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.Dashboard;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOne;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOne.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition.SelectFilterType;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo;
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
            SolutionShortlisted = new SolutionShortlisted(driver, commonActions);
            NoSolutionsFound = new NoSolutionsFound(driver, commonActions);
            SingleSolutionFound = new SingleSolutionFound(driver, commonActions);
            CompetitionTaskList = new CompetitionTaskList(driver, commonActions);
            CompetitionServiceRecipients = new CompetitionServiceRecipients(driver, commonActions);
            ContractLength = new ContractLength(driver, commonActions);
            AwardCriteria = new AwardCriteria(driver, commonActions);
            CalculatePrice = new CalculatePrice(driver, commonActions, factory);
            SolutionServicePrice = new SolutionServicePrice(driver, commonActions);
            SolutionServiceQuantity = new SolutionServiceQuantity(driver, commonActions);

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

        internal SolutionShortlisted SolutionShortlisted { get; }

        internal NoSolutionsFound NoSolutionsFound { get; }

        internal SingleSolutionFound SingleSolutionFound { get; }

        internal CompetitionTaskList CompetitionTaskList { get; }

        internal CompetitionServiceRecipients CompetitionServiceRecipients { get; }

        internal ContractLength ContractLength { get; }

        internal AwardCriteria AwardCriteria { get; }

        internal CalculatePrice CalculatePrice { get; }

        internal SolutionServicePrice SolutionServicePrice { get; }

        internal SolutionServiceQuantity SolutionServiceQuantity { get; }

        internal FilterType FilterType { get; set; }

        internal IWebDriver Driver { get; }

        public void StepOnePrepareCompetition(FilterType filterType, string competitionName, ServiceRecipientSelectionMode recipients = ServiceRecipientSelectionMode.None)
        {
            int selectedFilter = (int)filterType;
            SelectFilter.SelectFilterForNewCompetition(selectedFilter);

            ReviewFilter.ReviewYourFilter();

            StartCompetition.CreateCompetition(competitionName);
            int competitionId = CompetitionId();

            if (filterType == FilterType.NoResults)
            {
                NoSolutionsFound.NoSolutions();
            }
            else if (filterType == FilterType.SingleResult)
            {
                SingleSolutionFound.SingleSolution();
            }
            else
            {
                int filterId = GetFilterId(competitionId);

                if (filterId == 2)
                {
                    int solutions = NoOfSlutions();

                    SelectSolutions.AddSolutions(solutions);
                    SolutionShortlisted.SolutionNotIncludedInShortlisting();
                    SolutionShortlisted.ConfirmSolutions();
                }

                CompetitionTaskList.CompetitionServiceRecipientsTask();
                CompetitionServiceRecipients.AddCompetitionServiceRecipient(recipients);
                CompetitionServiceRecipients.ConfirmServiceReceipientsChanges();

                CompetitionTaskList.ContractLengthTask();
                ContractLength.CompetitionContractLength();
            }
        }

        public void StepTwoDefineCompetitionCriteria(CompetitionType competitiontype)
        {
            int competitionId = CompetitionId();

            var competitionsolutions = GetCompetitionSolutions(competitionId);

            CompetitionTaskList.AwardCriteriaTask();

            if (competitiontype == CompetitionType.PriceOnly)
            {
                AwardCriteria.PriceONly();
                CompetitionTaskList.CalculatePriceTask();

                foreach (var solution in competitionsolutions)
                {
                    if (HasTieredPrice(solution))
                    {
                        CalculatePrice.SelectPrice(solution);
                        SolutionServiceQuantity.AddSolutionQuantity(solution);
                    }
                    else
                    {
                        CalculatePrice.SolutionPrice(solution);
                        CalculatePrice.ConfirmSolutionPrice(solution);
                        SolutionServiceQuantity.AddSolutionQuantity(solution);
                    }

                    if (HasAdditionalService(competitionId,solution))
                    {
                        var competitionsolutionservices = GetCompetitionSolutionServices(competitionId, solution);

                        foreach (var servicdid in competitionsolutionservices)
                        {
                            if (HasTieredPrice(servicdid))
                            {
                                CalculatePrice.SelectAdditionalServicePrice(servicdid);
                                SolutionServiceQuantity.AddAdditionalServiceQuantity(servicdid);
                            }
                            else
                            {
                                CalculatePrice.ConfirmAdditionalServicePrice(servicdid);
                                SolutionServiceQuantity.AddAdditionalServiceQuantity(servicdid);
                            }
                        }
                    }

                    CalculatePrice.ConfirmPriceAndQuantity();
                }
            }

            CalculatePrice.ConfirmCalculatePrice();
        }

        private int CompetitionId()
        {
            var competitionurl = Driver.Url.Split("/");

            for (int i = 0; i < competitionurl.Length - 1; i++)
            {
                if (competitionurl[i] == "competitions" && i + 1 < competitionurl.Length)
                {
                    if (int.TryParse(competitionurl[i + 1], out int competitionId))
                    {
                        return competitionId;
                    }
                }
            }

            throw new InvalidOperationException("Unable to extract competition Id from the competition URL");
        }

        private int GetFilterId(int competitionId)
        {
            using var dbContext = Factory.DbContext;

            var result = dbContext.Competitions
                .Where(x => x.Id == competitionId)
                .Select(y => y.FilterId).First();

            return result;
        }

        private int NoOfSlutions()
        {
            return new Random().Next(2, 5);
        }

        private IEnumerable<CatalogueItemId> GetCompetitionSolutions(int competitionId)
        {
            using var dbContext = Factory.DbContext;

            var solutions = dbContext.Competitions
                .SelectMany(x => x.CompetitionSolutions).Where(y => y.CompetitionId == competitionId & y.IsShortlisted == true).ToList();

            var competitionsolutions = solutions.Select(x => x.SolutionId).ToList();

            return competitionsolutions;
        }

        private IEnumerable<CatalogueItemId> GetCompetitionSolutionServices(int competitionId, CatalogueItemId solutionId)
        {
            using var dbContext = Factory.DbContext;

            var competitionservices = dbContext.CompetitionSolutions
                .SelectMany(x => x.SolutionServices).Where(y => y.CompetitionId == competitionId && y.SolutionId == solutionId).ToList();

            return competitionservices.Select(x => x.ServiceId).ToList();
        }

        private bool HasAdditionalService(int competitionId, CatalogueItemId solutionId)
        {
            using var dbContext = Factory.DbContext;

            var competitionservices = dbContext.CompetitionSolutions
                .SelectMany(x => x.SolutionServices).Where(y => y.CompetitionId == competitionId && y.SolutionId == solutionId).ToList();
            return competitionservices.Count() > 0;
        }

        private bool HasTieredPrice(CatalogueItemId competitionservice)
        {
            using var dbContext = Factory.DbContext;

            var prices = dbContext.CatalogueItems
                .SelectMany(x => x.CataloguePrices).Where(y => y.CatalogueItemId == competitionservice);
            return prices.Count() > 1;
        }
    }
}
