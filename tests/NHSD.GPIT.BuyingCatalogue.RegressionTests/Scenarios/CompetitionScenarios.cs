using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo.NonPrice;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Scenarios
{
    public class CompetitionScenarios(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper) : BuyerTestBase(
        factory,
        typeof(BuyerDashboardController),
        nameof(BuyerDashboardController.Index),
        Parameters,
        testOutputHelper), IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "IB-QWO";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };

        [Fact]
        [Trait("Further Competition", "Multiple Results")]
        public void CompetitionForMultipleResultFilter()
        {
            string competitionName = "CompetitionForMultipleResultFilter";

            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, competitionName, ServiceRecipientSelectionMode.Multiple);
        }

        [Fact]
        [Trait("Further Competition", "No Results")]
        public void CompetitionForNoResultFilter()
        {
            string competitionName = "CompetitionForNoResultFilter";

            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.NoResults, competitionName);
        }

        [Fact]
        [Trait("Further Competition", "Single Results")]
        public void CompetitionForSingleResultFilter()
        {
            string competitionName = "CompetitionForSingleResultFilter";

            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.SingleResult, competitionName);
        }

        [Fact]
        [Trait("Further Competition", "Price Only")]
        public void CompetitionPriceOnly()
        {
            string competitionName = "CompetitionPriceOnly";

            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, competitionName, ServiceRecipientSelectionMode.Single);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceOnly);

            CompetitionPages.ViewResults();
        }

        [Fact]
        [Trait("Further Competition", "Competition to Order")]
        public void CompetitionOrderFromPriceOnlyCompetition()
        {
            string competitionName = "CompetitionOrderFromPriceOnlyCompetition";

            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, competitionName, ServiceRecipientSelectionMode.Single);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceOnly);

            CompetitionPages.ViewResults();

            CompetitionPages.CreateOrder();

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Further Competition", "Competition to Order")]
        public void CompetitionOrderFromPricAndNonPriceCompetition()
        {
            string competitionName = "CompetitionOrderFromPricAndNonPriceCompetition";

            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, competitionName, ServiceRecipientSelectionMode.Single);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceAndNonPriceElement, NonPriceElementType.Feature);

            CompetitionPages.ViewResults();

            CompetitionPages.CreateOrder();

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Further Competition", "Competition to Order")]
        public void CompetitionOrderForPriceOnlyMultipleResults()
        {
            string competitionName = "CompetitionOrderForPriceOnlyMultipleResults";

            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, competitionName, ServiceRecipientSelectionMode.Multiple);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceOnly);

            CompetitionPages.ViewMultipleResults();

            CompetitionPages.CreateOrder();

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Further Competition", "Price Only")]
        public void CompetitionPriceOnlyMultipleRecipients()
        {
            string competitionName = "CompetitionPriceOnlyMultipleRecipients";

            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, competitionName, ServiceRecipientSelectionMode.Multiple);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceOnly);

            CompetitionPages.ViewResults();
        }

        [Fact]
        [Trait("Further Competition", "Price Only")]
        public void CompetitionPriceOnlyMultipleResults()
        {
            string competitionName = "CompetitionPriceOnlyMultipleResults";

            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, competitionName, ServiceRecipientSelectionMode.Multiple);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceOnly);

            CompetitionPages.ViewMultipleResults();
        }

        [Fact]
        [Trait("Further Competition", "Price Only")]
        public void CompetitionPriceOnlyAllICBRecipients()
        {
            string competitionName = "CompetitionPriceOnlyAllICBRecipients";

            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, competitionName, ServiceRecipientSelectionMode.All);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceOnly);

            CompetitionPages.ViewResults();
        }

        [Fact]
        [Trait("Further Competition", "Price And Non Price")]
        public void CompetitionPricAndNonPriceElementFeature()
        {
            string competitionName = "CompetitionPricAndNonPriceElementFeature";

            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, competitionName, ServiceRecipientSelectionMode.Single);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceAndNonPriceElement, NonPriceElementType.Feature);

            CompetitionPages.ViewResults();
        }

        [Fact]
        [Trait("Further Competition", "Price And Non Price")]
        public void CompetitionPricAndNonPriceElementImplementation()
        {
            string competitionName = "CompetitionPricAndNonPriceElementImplementation";

            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, competitionName, ServiceRecipientSelectionMode.Single);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceAndNonPriceElement, NonPriceElementType.Implementation);

            CompetitionPages.ViewResults();
        }

        [Fact]
        [Trait("Further Competition", "Price And Non Price")]
        public void CompetitionPricAndNonPriceElementInteroperability()
        {
            string competitionName = "CompetitionPricAndNonPriceElementInteroperability";

            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, competitionName, ServiceRecipientSelectionMode.Single);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceAndNonPriceElement, NonPriceElementType.Interoperability);

            CompetitionPages.ViewResults();
        }

        [Fact]
        [Trait("Further Competition", "Price And Non Price")]
        public void CompetitionPricAndNonPriceElementServiceLevelAgreement()
        {
            string competitionName = "CompetitionPricAndNonPriceElementServiceLevelAgreement";

            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, competitionName, ServiceRecipientSelectionMode.Single, false);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceAndNonPriceElement, NonPriceElementType.ServiceLevelAgreement);

            CompetitionPages.ViewResults();
        }

        [Fact]
        [Trait("Further Competition", "Price And Non Price")]
        public void CompetitionAllNonPriceElements()
        {
            string competitionName = "CompetitionAllNonPriceElements";

            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, competitionName, ServiceRecipientSelectionMode.Single, false);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceAndNonPriceElement, NonPriceElementType.All);

            CompetitionPages.ViewResults();
        }
    }
}
