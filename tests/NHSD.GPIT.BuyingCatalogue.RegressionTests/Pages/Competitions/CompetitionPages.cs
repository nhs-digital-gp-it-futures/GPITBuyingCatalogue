using Microsoft.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.CompetitionToOrder;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.Dashboard;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOne;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOne.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition.SelectFilterType;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepOneCreateCompetition.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo.NonPrice;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.View_Result;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Dashboard;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepOne;
using OpenQA.Selenium;
using static Azure.Core.HttpHeader;

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
            SolutionServiceQuantity = new SolutionServiceQuantity(driver, commonActions);
            ViewCompetitionResults = new ViewCompetitionResults(driver, commonActions);
            AwardCriteriaWeightings = new AwardCriteriaWeightings(driver, commonActions);
            CompetitionNonPriceElements = new CompetitionNonPriceElements(driver, commonActions);
            NonPriceWeightings = new NonPriceWeightings(driver, commonActions);
            ReviewCompetitionCriteria = new ReviewCompetitionCriteria(driver, commonActions);
            CompareAndScore = new CompareAndScore(driver, commonActions);
            CreateOrderFromWinningSolution = new CreateOrderFromWinningSolution(driver, commonActions);
            TaskList = new TaskList(driver, commonActions);
            OrderingStepOne = new OrderingStepOne(driver, commonActions);
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

        internal SolutionServiceQuantity SolutionServiceQuantity { get; }

        internal ViewCompetitionResults ViewCompetitionResults { get; }

        internal AwardCriteriaWeightings AwardCriteriaWeightings { get; }

        internal CompetitionNonPriceElements CompetitionNonPriceElements { get; }

        internal NonPriceWeightings NonPriceWeightings { get; }

        internal ReviewCompetitionCriteria ReviewCompetitionCriteria { get; }

        internal CompareAndScore CompareAndScore { get; }

        internal CreateOrderFromWinningSolution CreateOrderFromWinningSolution { get; }

        internal TaskList TaskList { get; }

        internal OrderingStepOne OrderingStepOne { get; }

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

        public void StepTwoDefineCompetitionCriteria(CompetitionType competitiontype, NonPriceElementType elementtype = NonPriceElementType.Null)
        {
            int competitionId = CompetitionId();

            var competitionsolutions = GetCompetitionSolutions(competitionId);

            CompetitionTaskList.AwardCriteriaTask();

            if (competitiontype == CompetitionType.PriceOnly)
            {
                AwardCriteria.PriceONly();
            }
            else
            {
                AwardCriteria.PriceAndNonPrice();
                CompetitionTaskList.AwardCriteriaWeightings();
                AwardCriteriaWeightings.PriceNonPriceAwardCriteriaWeightings();
                CompetitionTaskList.NonPriceElements();
                CompetitionNonPriceElements.AddNonPriceElements(elementtype);
                switch (elementtype)
                {
                    case NonPriceElementType.All:
                        CompetitionNonPriceElements.AllNonPriceElementsReview();
                        break;
                    default:
                        CompetitionNonPriceElements.AddNonPriceElement();
                        break;
                }

                CompetitionTaskList.NonPriceWeightings();
                NonPriceWeightings.Weightings(elementtype);
                CompetitionTaskList.ReviewCompetitionCriteria();
                ReviewCompetitionCriteria.ReviewCriteria();
                CompetitionTaskList.CompareAndScoreLink();
                CompareAndScore.CompareAndScoreShortlistedSolutions(elementtype);
            }

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

                if (HasAdditionalService(competitionId, solution))
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

            CalculatePrice.ConfirmCalculatePrice();
        }

        public void ViewResults()
        {
            CompetitionTaskList.ViewResult();
            ViewCompetitionResults.ViewResults();
        }

        public void CreateOrder()
        {
            int competitionid = CompetitionId();
            var winningsolutiont = WinningResult(competitionid);
            CreateOrderFromWinningSolution.CreateOrderFromCompetition(winningsolutiont.ToString());
            TaskList.CallOffOrderingPartyContactDetailsTask();
            OrderingStepOne.AddCallOffOrderingPartyContactDetails();
            TaskList.CompetitionSupplierInformationAndContactDetailsTask();
            CreateOrderFromWinningSolution.ConfirmContact();
            TaskList.TimescalesForCallOffAgreementTask();
            CreateOrderFromWinningSolution.AddTimescaleForCallOffAgreement();
            TaskList.SelectPlannedDeliveryDatesTask();
            CreateOrderFromWinningSolution.CompetitionPlannedDeliveryDate();
            TaskList.SelectFundingSourcesTask();
            if (HasAdditionalService(competitionid, winningsolutiont))
            {
                var competitionsolutionservices = GetCompetitionSolutionServices(competitionid, winningsolutiont);
                var fundingservices = new List<CatalogueItemId>();
                fundingservices.Add(winningsolutiont);
                foreach (var service in competitionsolutionservices)
                {
                    fundingservices.Add(service);
                }

                CreateOrderFromWinningSolution.FundingSources(fundingservices);
            }
        }

        public void ViewMultipleResults()
        {
            int competitionid = CompetitionId();
            CompetitionTaskList.ViewResult();
            MultipleResults(competitionid);
        }

        private int CompetitionId()
        {
            const string lookupString = "competitions/";

            var url = Driver.Url.AsSpan();
            var beginningIndex = url.IndexOf(lookupString, StringComparison.Ordinal) + lookupString.Length;
            var trimmedUrl = url.Slice(beginningIndex);
            var endIndex = trimmedUrl.IndexOf('/');
            var competitionIdLength = endIndex != -1 ? endIndex : trimmedUrl.Length;
            var competitionId = trimmedUrl.Slice(0, competitionIdLength).ToString();

            if (!int.TryParse(competitionId, out var parsedCompetitionId)) throw new InvalidOperationException("Unable to extract competition Id from the competition URL");

            return parsedCompetitionId;
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

        private CatalogueItemId WinningResult(int competitionId)
        {
            using var dbContext = Factory.DbContext;

            var solutions = dbContext.Competitions
                .SelectMany(x => x.CompetitionSolutions)
                .Where(y => y.CompetitionId == competitionId && y.IsWinningSolution && y.IsShortlisted);

            var winningSolution = solutions.Select(x => x.SolutionId).ToList();

            if (winningSolution.Any())
            {
                var winningSolutionResult = winningSolution.First();
                return winningSolutionResult;
            }
            else
            {
                throw new ArgumentNullException(nameof(competitionId));
            }
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

        private void MultipleResults(int competitionId)
        {
            using var dbContext = Factory.DbContext;

            var solutions = dbContext.Competitions
                .SelectMany(x => x.CompetitionSolutions)
                .Where(y => y.CompetitionId == competitionId && !y.IsWinningSolution && y.IsShortlisted)
                .ToList();

            if (solutions.Any())
            {
                var nextWinningSolution = solutions.First();
                nextWinningSolution.IsWinningSolution = true;

                dbContext.SaveChanges();

                ViewCompetitionResults.ViewMultipleWinningResults();
            }
            else
            {
                throw new ArgumentNullException(nameof(competitionId));
            }
        }
    }
}
