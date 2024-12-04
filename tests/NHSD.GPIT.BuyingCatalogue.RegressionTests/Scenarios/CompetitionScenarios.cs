using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo.NonPrice;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Scenarios
{
    public class CompetitionScenarios(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper) : BuyerTestBase(
        factory,
        testOutputHelper), IClassFixture<LocalWebApplicationFactory>
    {
        [Fact]
        [Trait("Further Competition", "Multiple Results")]
        public void CompetitionForMultipleResultFilter()
        {
            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, ServiceRecipientSelectionMode.Multiple);
        }

        [Fact]
        [Trait("Further Competition", "No Results")]
        public void CompetitionForNoResultFilter()
        {
            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.NoResults);
        }

        [Fact]
        [Trait("Further Competition", "Single Results")]
        public void CompetitionForSingleResultFilter()
        {
            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.SingleResult);
        }

        [Fact]
        [Trait("Further Competition", "Price Only")]
        public void CompetitionPriceOnly()
        {
            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, ServiceRecipientSelectionMode.Single);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceOnly);

            CompetitionPages.ViewResults();
        }

        [Fact]
        [Trait("Further Competition", "Competition to Order")]
        public void CompetitionOrderFromPriceOnlyCompetition()
        {
            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, ServiceRecipientSelectionMode.Single);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceOnly);

            CompetitionPages.ViewResults();

            CompetitionPages.CreateOrder();

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Further Competition", "Competition to Order")]
        public void CompetitionOrderFromPriceAndNonPriceCompetition()
        {
            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, ServiceRecipientSelectionMode.Single);

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
            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, ServiceRecipientSelectionMode.Multiple);

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
            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, ServiceRecipientSelectionMode.Multiple);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceOnly);

            CompetitionPages.ViewResults();
        }

        [Fact]
        [Trait("Further Competition", "Price Only")]
        public void CompetitionPriceOnlyMultipleResults()
        {
            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, ServiceRecipientSelectionMode.Multiple);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceOnly);

            CompetitionPages.ViewMultipleResults();
        }

        [Fact]
        [Trait("Further Competition", "Price Only")]
        public void CompetitionPriceOnlyAllICBRecipients()
        {
            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, ServiceRecipientSelectionMode.All);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceOnly);

            CompetitionPages.ViewResults();
        }

        [Fact]
        [Trait("Further Competition", "Price And Non Price")]
        public void CompetitionPriceAndNonPriceElementFeature()
        {
            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, ServiceRecipientSelectionMode.Single);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceAndNonPriceElement, NonPriceElementType.Feature);

            CompetitionPages.ViewResults();
        }

        [Fact]
        [Trait("Further Competition", "Price And Non Price")]
        public void CompetitionPriceAndNonPriceElementImplementation()
        {
            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, ServiceRecipientSelectionMode.Single);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceAndNonPriceElement, NonPriceElementType.Implementation);

            CompetitionPages.ViewResults();
        }

        [Fact]
        [Trait("Further Competition", "Price And Non Price")]
        public void CompetitionPriceAndNonPriceElementInteroperability()
        {
            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, ServiceRecipientSelectionMode.Single);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceAndNonPriceElement, NonPriceElementType.Interoperability);

            CompetitionPages.ViewResults();
        }

        [Fact]
        [Trait("Further Competition", "Price And Non Price")]
        public void CompetitionPriceAndNonPriceElementServiceLevelAgreement()
        {
            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, ServiceRecipientSelectionMode.Single);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceAndNonPriceElement, NonPriceElementType.ServiceLevelAgreement);

            CompetitionPages.ViewResults();
        }

        [Fact]
        [Trait("Further Competition", "Price And Non Price")]
        public void CompetitionAllNonPriceElements()
        {
            CompetitionPages.CompetitionDashboard.CompetitionTriage();

            CompetitionPages.BeforeYouStart.ReadyToStart();

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, ServiceRecipientSelectionMode.Single);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceAndNonPriceElement, NonPriceElementType.All);

            CompetitionPages.ViewResults();
        }
    }
}
