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
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditCatalogueSolution()
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
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditCatalogueSolution()
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
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_EditCatalogueSolution()
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

        [Fact]
        public void OrderAssociatedServiceOnly_EditAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: NewSolutionName, associatedService: AssociatedServiceNameForWebGP);

            OrderingPages.EditAssociatedServiceOnly(NewSolutionName, NewAssociatedServiceName, AssociatedServiceNameForWebGP);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionUnder40K_EditCatalogueSolutionServiceRecipient()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditCatalogueSolutionServiceRecipient(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionUnder40K_EditCatalogueSolutionPrice()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditCatalogueSolutionPrice(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionUnder40K_EditCatalogueSolutionQuantity()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditCatalogueItemQuantity(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalServiceRecipient()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.EditAdditionalServiceRecipient(NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalServicePrice()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.EditAdditionalServicePrice(NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalServiceQuantity()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.EditCatalogueItemQuantity(NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedServiceRecipient()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditAssociatedServiceRecipient(NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedServicePrice()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditAssociatedServicePrice(NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedServiceQuantity()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditCatalogueItemQuantity(NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderAssociatedServiceOnly_EditAssociatedServiceRecipients()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditAssociatedServiceOnlyServiceRecipients(NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderAssociatedServiceOnly_EditAssociatedServicePrice()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditAssociatedServiceOnlyPrice(NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderAssociatedServiceOnly_EditAssociatedServiceQuantity()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditCatalogueItemQuantity(NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAdditionalServicesUnder40K_MultipleAdditionalServices()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalServices: new List<string>()
                {
                    AdditionalServiceName,
                    NewAdditionalServiceName,
                });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAdditionalAndAssociatedServicesUnder40K_MultipleAdditionalServices_OneAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalServices: new List<string>()
                {
                    AdditionalServiceName,
                    NewAdditionalServiceName,
                },
                AssociatedServiceNameForWebGP);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_MultipleAssociatedServices_OneAdditionalService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalService: AdditionalServiceName,
                associatedServices: new List<string> { NewAssociatedServiceName, AssociatedServiceNameForWebGP });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_MultipleAssociatedServices()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalService: string.Empty,
                associatedServices: new List<string> { NewAssociatedServiceName, AssociatedServiceNameForWebGP });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_MultipleAdditionalServices_MultipleAssociatedServices()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalServices: new List<string>()
                {
                    AdditionalServiceName,
                    NewAdditionalServiceName,
                },
                associatedServices: new List<string> { NewAssociatedServiceName, AssociatedServiceNameForWebGP });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithAssociatedServiceOnly_MultipleAssociatedServices()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalServices: null,
                associatedServices: new List<string> { NewAssociatedServiceName, AssociatedServiceNameForWebGP });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionUnder40K_MultipleServiceRecipients()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                multipleServiceRecipients: true);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_MultipleServiceRecipients()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalService: AdditionalServiceName,
                multipleServiceRecipients: true);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_MultipleServiceRecipients()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                associatedService: AssociatedServiceNameForWebGP,
                multipleServiceRecipients: true);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_MultipleServiceRecipients()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, orderTriage: OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalService: AdditionalServiceName,
                associatedService: AssociatedServiceNameForWebGP,
                multipleServiceRecipients: true);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithAssociatedServiceOnly_MultipleServiceRecipients()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                associatedService: AssociatedServiceNameForWebGP,
                multipleServiceRecipients: true);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderAssociatedServiceOnly_MultipleAssociatedServices_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.EditCatalogueSolution(newSolutionName: NewSolutionName, newAdditionalServiceName: string.Empty, newAssociatedServices: new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_MultipleAdditionalServices_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, new List<string> { AdditionalServiceName, NewAdditionalServiceName }, newAssociatedService: string.Empty);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_MultipleAssociatedServices_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceName: string.Empty, new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_MultipleAdditional_MultipleAssociatedServices_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, new List<string> { AdditionalServiceName, NewAdditionalServiceName }, new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionUnder40K_MultipleServiceRecipients_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, multipleServiceRecipients: true);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_MultipleServiceRecipients_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceNames: new List<string> { NewAdditionalServiceName, AdditionalServiceName }, newAssociatedService: string.Empty, multipleServiceRecipients: true);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_MultipleServiceRecipients_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceNames: null, newAssociatedServices: new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName }, multipleServiceRecipients: true);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_MultipleServiceRecipients_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceNames: new List<string> { NewAdditionalServiceName }, newAssociatedServices: new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName }, multipleServiceRecipients: true);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithAssociatedServiceOnly_MultipleAssociatedServices_MultipleServiceRecipients_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: SolutionName,
                associatedService: AssociatedServiceName,
                multipleServiceRecipients: true);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceName: string.Empty, new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName }, true);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_MultipleAdditionalServices_OneAssociatedService_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceNames: new List<string> { NewAdditionalServiceName, AdditionalServiceName }, AssociatedServiceNameForWebGP, multipleServiceRecipients: true);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionUnder40K_MultipleAdditionalServices_EditAdditionalService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditAdditionalService(NewSolutionName, new List<string> { NewAdditionalServiceName, AdditionalServiceName });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_MultipleAdditionalServices_EditAdditionalService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, "Document Management", multipleServiceRecipients: false);

            OrderingPages.EditAdditionalService(NewSolutionName, new List<string> { NewAdditionalServiceName, AdditionalServiceName }, oldAdditionalService: "Document Management");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionUnder40K_MultipleAssociatedServices_EditAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditAssociatedService(NewSolutionName, new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName }, string.Empty, string.Empty);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_MultipleAssociatedServices_EditAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.EditAssociatedService(NewSolutionName, new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName }, oldAssociatedServiceName: "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        public void OrderAssociatedServiceOnly_MultipleAssociatedServices_EditAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: NewSolutionName, associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.EditAssociatedServiceOnly(NewSolutionName, new List<string> { NewAssociatedServiceName, AssociatedServiceNameForWebGP }, "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }
    }
}
