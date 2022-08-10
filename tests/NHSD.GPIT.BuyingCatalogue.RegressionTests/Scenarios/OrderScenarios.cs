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
        private const string NewSolutionName = "Emis Web GP";
        private const string NewAdditionalServiceName = "EMIS Mobile";
        private const string NewAssociatedServiceName = "Automated Arrivals – Specialist Cabling";
        private const string AssociatedServiceNameForWebGP = "Engineering";

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

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
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

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
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

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
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

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
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

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
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

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
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

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
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

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
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

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
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

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
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

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
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

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderAssociatedServiceOnly()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_WithAdditionalService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_WithAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(newSolutionName: NewSolutionName, newAssociatedService: NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_WithAdditionalAndAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(newSolutionName: NewSolutionName, newAdditionalServiceName: NewAdditionalServiceName, newAssociatedService: NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderAssociatedServiceOnly_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.EditCatalogueSolution(newSolutionName: NewSolutionName, newAssociatedService: NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionUnder40K_SolutionDoesNotHaveAdditionalService_EditAdditionalService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditAdditionalService(SolutionName, NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionUnder40K_EditAdditionalService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditAdditionalService(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, AdditionalServiceName);

            OrderingPages.EditAdditionalService(NewSolutionName, NewAdditionalServiceName, oldAdditionalService: AdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_EditAdditionalService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, AdditionalServiceName, NewAssociatedServiceName);

            OrderingPages.EditAdditionalService(NewSolutionName, NewAdditionalServiceName, oldAdditionalService: AdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionUnder40K_EditAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditAssociatedService(NewSolutionName, NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.EditAssociatedService(NewSolutionName, NewAssociatedServiceName, NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_EditAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, NewAdditionalServiceName, AssociatedServiceNameForWebGP);

            OrderingPages.EditAssociatedService(NewSolutionName, NewAssociatedServiceName, NewAdditionalServiceName, AssociatedServiceNameForWebGP);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

      /*  [Fact]
        public void OrderAssociatedServiceOnly_EditAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.EditCatalogueSolution(newSolutionName: NewSolutionName, newAssociatedService: NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }*/
    }
}
