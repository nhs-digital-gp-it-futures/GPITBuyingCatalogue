using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Scenarios
{
    public class OrderScenarios : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";

        private static readonly Dictionary<string, string> Parameters =
            new()
            {
                { nameof(InternalOrgId), InternalOrgId },
            };

        public OrderScenarios(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
           : base(factory, typeof(DashboardController), nameof(DashboardController.Organisation), Parameters, testOutputHelper)
        {
        }

        [Fact]
        public void FirstTest_DoesThing()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectTriageOrderValue(EntityFramework.Ordering.Models.OrderTriageValue.Over250K);

            OrderingPages.OrderingTriage.SelectIdentifiedOrder();

            OrderingPages.OrderingTriage.ReadyToStart();
        }

        [Fact]
        public void OrderWithSolutionUnder40K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(EntityFramework.Ordering.Models.OrderTriageValue.Under40K);

            OrderingPages.TaskList.OrderDescriptionTask();
            OrderingPages.OrderingStepOne.AddOrderDescription();

            OrderingPages.TaskList.CallOffOrderingPartyContactDetailsTask();
            OrderingPages.OrderingStepOne.AddCallOffOrderingPartyContactDetails();

            OrderingPages.TaskList.SupplierInformationTask();
            OrderingPages.OrderingStepTwo.AddSupplierInformation();

            OrderingPages.TaskList.SupplierContactDetailsTask();
            OrderingPages.OrderingStepTwo.AddSupplierContactDetails();

            OrderingPages.TaskList.TimescalesForCallOffAgreementTask();
            OrderingPages.OrderingStepTwo.TimescalesForCallOffAgreementDetails();

            OrderingPages.TaskList.SelectSolutionTask();
            OrderingPages.SelectCatalogueSolution.SelectCatalogueSolution_SelectSolution();
            OrderingPages.SelectCatalogueSolutionRecipients.SelectCatalogueSolutionRecipients_SelectionMade();
            OrderingPages.Price.CatalogueSolutionConfirmPrice();
            OrderingPages.Quantity.SelectServiceRecipientQuantity_CatalogueSolution();
            OrderingPages.ReviewSolutions.ReviewSolution();
        }
    }
}
