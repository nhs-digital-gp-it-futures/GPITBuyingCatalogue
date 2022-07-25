using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Scenarios
{
    public class OrderScenarios : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "CG-03F";

        private const string SupplierName = "EMIS Health";
        private const string SolutionName = "Anywhere Consult";
        private const string AssociatedServiceName = "Anywhere Consult – Integrated Device";
        private const string AdditionalServiceName = "Automated Arrivals";

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

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectTriageOrderValue(OrderTriageValue.Over250K);

            OrderingPages.OrderingTriage.SelectIdentifiedOrder();

            OrderingPages.StartOrder.ReadyToStart();
        }

        [Fact]
        public void OrderWithSolutionUnder40K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.StepThreeReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAssociatedServiceUnder40K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.StepThreeReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAdditionalServiceUnder40K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals");

            OrderingPages.StepThreeReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals", associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeReviewAndCompleteOrder();
        }

        [Fact]
        public void CatalogueSolutionOnlyBetween40KTo250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Between40KTo250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Between40KTo250K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.StepThreeReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAdditionalServiceBetween40Kand250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Between40KTo250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Between40KTo250K);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals");

            OrderingPages.StepThreeReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAssociatedServiceBetween40Kand250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Between40KTo250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Between40KTo250K);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.StepThreeReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAdditionalAndAssociatedServiceBetween40Kand250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Between40KTo250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Between40KTo250K);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals", associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeReviewAndCompleteOrder();
        }

        [Fact]
        public void CatalogueSolutionOnlyOver250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Over250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Over250K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.StepThreeReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAssociatedServiceOver250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Over250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Over250K);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.StepThreeReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAdditionalServiceOver250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Over250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Over250K);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals");

            OrderingPages.StepThreeReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAdditionalAndAssociatedServiceOver250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Over250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Over250K);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals", associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderAssociatedServiceOnly()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.StepThreeReviewAndCompleteOrder();
        }
    }
}
