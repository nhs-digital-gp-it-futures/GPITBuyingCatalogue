using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Competitions.StepTwo.NonPrice;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.OrderType;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Controllers;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Scenarios
{
    public class OrderScenarios : BuyerTestBase, IClassFixture<LocalWebApplicationFactory>
    {
        private const string InternalOrgId = "IB-QWO";
        private const string FileName = "valid_service_recipients.csv";
        private const string SupplierName = "EMIS Health";
        private const string SolutionName = "Anywhere Consult";
        private const string SolutionWithMultipleFrameworks = "Video Consult";
        private const string SolutionForLocalfundingonly = "Online and Video Consult";
        private const string AssociatedServiceName = "Anywhere Consult – Integrated Device";
        private const string AssociatedServiceMerger = "Practice Merge";
        private const string AssociatedServiceSplit = "Practice Reorganisation";
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
           : base(factory, typeof(BuyerDashboardController), nameof(BuyerDashboardController.Index), Parameters, testOutputHelper)
        {
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionUnder40K()
        {
            string orderDescription = "OrderWithSolutionUnder40K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Framework", "Order")]
        public void LocalFundingOnlyFrameworksOrderWithSolutionUnder40K()
        {
            string orderDescription = "LocalFundingOnlyFrameworksOrderWithSolutionUnder40K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionForLocalfundingonly);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Framework", "Order")]
        public void MultipleFrameworksOrderWithSolutionUnder40K()
        {
            string orderDescription = "MultipleFrameworksOrderWithSolutionUnder40K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionWithMultipleFrameworks);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K()
        {
            string orderDescription = "OrderWithSolutionAndAssociatedServiceUnder40K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K()
        {
            string orderDescription = "OrderWithSolutionAndAdditionalServiceUnder40K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K()
        {
            string orderDescription = "OrderWithSolutionAdditionalAndAssociatedServiceUnder40K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals", associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void EditPlannedDeliveryDateOrderWithSolutionAdditionalAndAssociatedServiceUnder40K()
        {
            string orderDescription = "EditPlannedDeliveryDateOrderWithSolutionAdditionalAndAssociatedServiceUnder40K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals", associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.EditPlannedDeliveryDate("Emis Web GP", additionalService: "Automated Arrivals", associatedService: "Automated Arrivals – Engineering Half Day", true);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void CatalogueSolutionOnlyBetween40KTo250K()
        {
            string orderDescription = "CatalogueSolutionOnlyBetween40KTo250K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Between40KTo250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Between40KTo250K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();

            OrderingPages.StepFiveAmendOrder();
        }

        [Fact]
        [Trait("Amend Order Journey", "Order")]
        public void Amend_CatalogueSolution()
        {
            string orderDescription = "Amend_CatalogueSolution";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();

            OrderingPages.StepFiveAmendOrder();

            OrderingPages.AmendSolutionsAndServices(NewSolutionName);
        }

        [Fact]
        [Trait("Amend Order Journey", "Order")]
        public void Amend_CatalogueSolution_multiple_servicereceipients()
        {
            string orderDescription = "Amend_CatalogueSolution_multiple_servicereceipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();

            OrderingPages.StepFiveAmendOrder();

            OrderingPages.AmendSolutionsAndServices(NewSolutionName, multipleServiceRecipients: 3);
        }

        [Fact]
        [Trait("Amend Order Journey", "Order")]
        public void Amend_CatalogueSolution_import_servicereceipients()
        {
            string orderDescription = "Amend_CatalogueSolution_import_servicereceipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();

            OrderingPages.StepFiveAmendOrder();

            OrderingPages.AmendSolutionsAndServices(
                NewSolutionName,
                importServiceRecipients: true,
                fileName: FileName);
        }

        [Fact]
        [Trait("Amend Order Journey", "Order")]
        public void AmendCatalogueSolutionsAndAdditionalService()
        {
            string orderDescription = "AmendCatalogueSolutionsAndAdditionalService";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();

            OrderingPages.StepFiveAmendOrder();

            OrderingPages.AmendSolutionsAndServices(NewSolutionName, additionalService: "Automated Arrivals");
        }

        [Fact]
        [Trait("Amend Order Journey", "Order")]
        public void AmendMultipleAdditionalService()
        {
            string orderDescription = "AmendMultipleAdditionalService";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();

            OrderingPages.StepFiveAmendOrder();

            OrderingPages.AmendAddSolutionsAndServices(
                NewSolutionName,
                additionalServices: new List<string>()
                {
                    AdditionalServiceName,
                    NewAdditionalServiceName,
                });
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void CatalogueSolutionOnlyWithNewSupplierContactBetween40KTo250K()
        {
            string orderDescription = "CatalogueSolutionOnlyWithNewSupplierContactBetween40KTo250K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Between40KTo250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, true, OrderTriageValue.Between40KTo250K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServiceBetween40Kand250K()
        {
            string orderDescription = "OrderWithSolutionAndAdditionalServiceBetween40Kand250K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Between40KTo250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Between40KTo250K);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServiceBetween40Kand250KStepThreeCustomRoute()
        {
            string orderDescription = "OrderWithSolutionAndAdditionalServiceBetween40Kand250K StepThreeCustomRoute";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Between40KTo250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Between40KTo250K);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAssociatedServiceBetween40Kand250K()
        {
            string orderDescription = "OrderWithSolutionAndAssociatedServiceBetween40Kand250K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Between40KTo250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Between40KTo250K);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceBetween40Kand250K()
        {
            string orderDescription = "OrderWithSolutionAdditionalAndAssociatedServiceBetween40Kand250K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Between40KTo250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Between40KTo250K);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals", associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeCompleteContract(false);

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceBetween40Kand250KStepThreeCustomRoute()
        {
            string orderDescription = "OrderWithSolutionAdditionalAndAssociatedServiceBetween40Kand250K StepThreeCustomRoute";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Between40KTo250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Between40KTo250K);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals", associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeCompleteContract(false);

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void CatalogueSolutionOnlyOver250K_AllserviceRecipients()
        {
            string orderDescription = "CatalogueSolutionOnlyOver250K_AllserviceRecipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Over250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Over250K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName, allServiceRecipients: true);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void CatalogueSolutionOnlyOver250K()
        {
            string orderDescription = "CatalogueSolutionOnlyOver250K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Over250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Over250K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void CatalogueSolutionOnlyOver250KStepThreeCustomRoute()
        {
            string orderDescription = "CatalogueSolutionOnlyOver250K Step Three Custom Route";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Over250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Over250K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.StepThreeCompleteContract(false);

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAssociatedServiceOver250K()
        {
            string orderDescription = "OrderWithSolutionAndAssociatedServiceOver250K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Over250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Over250K);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServiceOver250K()
        {
            string orderDescription = "OrderWithSolutionAndAdditionalServiceOver250K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Over250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Over250K);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceOver250K()
        {
            string orderDescription = "OrderWithSolutionAdditionalAndAssociatedServiceOver250K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Over250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Over250K);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals", associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Associated Service Only Journey", "Order")]
        public void OrderAssociatedServiceOnly()
        {
            string orderDescription = "OrderAssociatedServiceOnly";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService, AssociatedServiceType.AssociatedServiceOther);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Associated Service Only Journey", "Split Order")]
        public void OrderSplitAssociatedServiceOnly()
        {
            string orderDescription = "OrderSplitAssociatedServiceOnly";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService, AssociatedServiceType.AssociatedServiceSplit);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, itemType: CatalogueItemType.AssociatedService, associatedServiceType: AssociatedServiceType.AssociatedServiceSplit);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceSplit, multipleServiceRecipients: 3);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Associated Service Only Journey", "Merger Order")]
        public void OrderMergerAssociatedServiceOnly()
        {
            string orderDescription = "OrderMergerAssociatedServiceOnly";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService, AssociatedServiceType.AssociatedServiceMerger);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, itemType: CatalogueItemType.AssociatedService, associatedServiceType: AssociatedServiceType.AssociatedServiceMerger);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceMerger, multipleServiceRecipients: 3);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Associated Service Only Journey", "Order")]
        public void EditPlannedDeliveryDateOrderAssociatedServiceOnly()
        {
            string orderDescription = "EditPlannedDeliveryDateOrderAssociatedServiceOnly";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.EditPlannedDeliveryDate("Anywhere Consult", "Anywhere Consult – Integrated Device", " ", true);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Associated Service Only Journey", "Order")]
        public void OrderAssociatedServiceOnlyWithStepThreeCustomRoute()
        {
            string orderDescription = "OrderAssociatedServiceOnlyWithStepThreeCustomRoute";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.StepThreeContractAssociatedServices(false);

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Catalogue Solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution()
        {
            string orderDescription = "OrderWithSolutionUnder40K_EditCatalogueSolution";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Catalogue Solution", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditCatalogueSolution()
        {
            string orderDescription = "OrderWithSolutionAndAdditionalServiceUnder40K_EditCatalogueSolution";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Catalogue Solution", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditCatalogueSolution()
        {
            string orderDescription = "OrderWithSolutionAndAssociatedServiceUnder40K_EditCatalogueSolution";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(newSolutionName: NewSolutionName, newAssociatedService: NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Catalogue Solution", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_EditCatalogueSolution()
        {
            string orderDescription = "OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_EditCatalogueSolution";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(newSolutionName: NewSolutionName, newAdditionalServiceName: NewAdditionalServiceName, newAssociatedService: NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Catalogue Solution", "Order")]
        public void OrderAssociatedServiceOnly_EditCatalogueSolution()
        {
            string orderDescription = "OrderAssociatedServiceOnly_EditCatalogueSolution";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.EditCatalogueSolution(newSolutionName: NewSolutionName, newAssociatedService: NewAssociatedServiceName);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Additional Service", "Order")]
        public void OrderWithSolutionUnder40K_SolutionDoesNotHaveAdditionalService_EditAdditionalService()
        {
            string orderDescription = "OrderWithSolutionUnder40K_SolutionDoesNotHaveAdditionalService_EditAdditionalService";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditAdditionalService(SolutionName, NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Additional Service", "Order")]
        public void OrderWithSolutionUnder40K_EditAdditionalService()
        {
            string orderDescription = "OrderWithSolutionUnder40K_EditAdditionalService";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditAdditionalService(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Additional Service", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalService()
        {
            string orderDescription = "OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalService";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, AdditionalServiceName);

            OrderingPages.EditAdditionalService(NewSolutionName, NewAdditionalServiceName, oldAdditionalService: AdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Additional Service", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditAdditionalService()
        {
            string orderDescription = "OrderWithSolutionAndAssociatedServiceUnder40K_EditAdditionalService";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditAdditionalService(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Additional Service", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_EditAdditionalService()
        {
            string orderDescription = "OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_EditAdditionalService";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, AdditionalServiceName, NewAssociatedServiceName);

            OrderingPages.EditAdditionalService(NewSolutionName, NewAdditionalServiceName, oldAdditionalService: AdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Associated Service", "Order")]
        public void OrderWithSolutionUnder40K_EditAssociatedService()
        {
            string orderDescription = "OrderWithSolutionUnder40K_EditAssociatedService";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditAssociatedService(NewSolutionName, NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Associated Service", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAssociatedService()
        {
            string orderDescription = "OrderWithSolutionAndAdditionalServiceUnder40K_EditAssociatedService";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.EditAssociatedService(NewSolutionName, NewAssociatedServiceName, NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Associated Service", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedService()
        {
            string orderDescription = "OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedService";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, associatedService: AssociatedServiceNameForWebGP);

            OrderingPages.EditAssociatedService(NewSolutionName, NewAssociatedServiceName, oldAssociatedServiceName: AssociatedServiceNameForWebGP);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Associated Service", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_EditAssociatedService()
        {
            string orderDescription = "OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_EditAssociatedService";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, NewAdditionalServiceName, AssociatedServiceNameForWebGP);

            OrderingPages.EditAssociatedService(NewSolutionName, NewAssociatedServiceName, NewAdditionalServiceName, AssociatedServiceNameForWebGP);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Associated Service", "Order")]
        public void OrderAssociatedServiceOnly_EditAssociatedService()
        {
            string orderDescription = "OrderAssociatedServiceOnly_EditAssociatedService";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: NewSolutionName, associatedService: AssociatedServiceNameForWebGP);

            OrderingPages.EditAssociatedServiceOnly(NewSolutionName, NewAssociatedServiceName, AssociatedServiceNameForWebGP);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolutionServiceRecipient()
        {
            string orderDescription = "OrderWithSolutionUnder40K_EditCatalogueSolutionServiceRecipient";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditCatalogueSolutionServiceRecipient(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolutionPrice()
        {
            string orderDescription = "OrderWithSolutionUnder40K_EditCatalogueSolutionPrice";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditCatalogueSolutionPrice(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolutionQuantity()
        {
            string orderDescription = "OrderWithSolutionUnder40K_EditCatalogueSolutionQuantity";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditCatalogueItemQuantity(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalServiceRecipient()
        {
            string orderDescription = "OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalServiceRecipient";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.EditAdditionalServiceRecipient(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalServicePrice()
        {
            string orderDescription = "OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalServicePrice";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.EditAdditionalServicePrice(NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalServiceQuantity()
        {
            string orderDescription = "OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalServiceQuantity";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.EditCatalogueItemQuantity(NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedServiceRecipient()
        {
            string orderDescription = "OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedServiceRecipient";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditAssociatedServiceRecipient(NewSolutionName, NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedServicePrice()
        {
            string orderDescription = "OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedServicePrice";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditAssociatedServicePrice(NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedServiceQuantity()
        {
            string orderDescription = "OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedServiceQuantity";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditCatalogueItemQuantity(NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderAssociatedServiceOnly_EditAssociatedServiceRecipients()
        {
            string orderDescription = "OrderAssociatedServiceOnly_EditAssociatedServiceRecipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditAssociatedServiceOnlyServiceRecipients(NewAssociatedServiceName);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderAssociatedServiceOnly_EditAssociatedServicePrice()
        {
            string orderDescription = "OrderAssociatedServiceOnly_EditAssociatedServicePrice";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditAssociatedServiceOnlyPrice(NewAssociatedServiceName);

            OrderingPages.StepThreeContractAssociatedServices(false);

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderAssociatedServiceOnly_EditAssociatedServiceQuantity()
        {
            string orderDescription = "OrderAssociatedServiceOnly_EditAssociatedServiceQuantity";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditCatalogueItemQuantity(NewAssociatedServiceName);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServicesUnder40K_MultipleAdditionalServices()
        {
            string orderDescription = "OrderWithSolutionAndAdditionalServicesUnder40K_MultipleAdditionalServices";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

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
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServicesUnder40K_MultipleAdditionalServices_OneAssociatedService()
        {
            string orderDescription = "OrderWithSolutionAdditionalAndAssociatedServicesUnder40K_MultipleAdditionalServices_OneAssociatedService";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

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
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_MultipleAssociatedServices_OneAdditionalService()
        {
            string orderDescription = "OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_MultipleAssociatedServices_OneAdditionalService";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalService: AdditionalServiceName,
                associatedServices: new List<string> { NewAssociatedServiceName, AssociatedServiceNameForWebGP });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_MultipleAssociatedServices()
        {
            string orderDescription = "OrderWithSolutionAndAssociatedServiceUnder40K_MultipleAssociatedServices";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalService: string.Empty,
                associatedServices: new List<string> { NewAssociatedServiceName, AssociatedServiceNameForWebGP });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_MultipleAdditionalServices_MultipleAssociatedServices()
        {
            string orderDescription = "OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_MultipleAdditionalServices_MultipleAssociatedServices";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

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
        [Trait("Associated Service Only Journey", "Order")]
        public void OrderWithAssociatedServiceOnly_MultipleAssociatedServices()
        {
            string orderDescription = "OrderWithAssociatedServiceOnly_MultipleAssociatedServices";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalServices: null,
                associatedServices: new List<string> { NewAssociatedServiceName, AssociatedServiceNameForWebGP });

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Associated Service Only Journey", "Split Order")]
        public void OrderWithSolutionUnder40K_MultipleServiceRecipients()
        {
            string orderDescription = "OrderWithSolutionUnder40K_MultipleServiceRecipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                multipleServiceRecipients: 3);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionUnder40K_ImportServiceRecipients()
        {
            string orderDescription = "OrderWithSolutionUnder40K_ImportServiceRecipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                importServiceRecipients: true,
                fileName: FileName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_MultipleServiceRecipients()
        {
            string orderDescription = "OrderWithSolutionAndAdditionalServiceUnder40K_MultipleServiceRecipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalService: AdditionalServiceName,
                multipleServiceRecipients: 3);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_ImportServiceRecipients()
        {
            string orderDescription = "OrderWithSolutionAndAdditionalServiceUnder40K_ImportServiceRecipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalService: AdditionalServiceName,
                importServiceRecipients: true,
                fileName: FileName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Associated Service Only Journey", "Split Order")]
        public void OrderAssociatedServiceOnly_ImportServiceRecipients()
        {
            string orderDescription = "OrderAssociatedServiceOnly_ImportServiceRecipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: SolutionName,
                associatedService: AssociatedServiceName,
                importServiceRecipients: true,
                fileName: FileName);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_MultipleServiceRecipients()
        {
            string orderDescription = "OrderWithSolutionAndAssociatedServiceUnder40K_MultipleServiceRecipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                associatedService: AssociatedServiceNameForWebGP,
                multipleServiceRecipients: 3);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_ImportServiceRecipients()
        {
            string orderDescription = "OrderWithSolutionAndAssociatedServiceUnder40K_ImportServiceRecipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                associatedService: AssociatedServiceNameForWebGP,
                multipleServiceRecipients: 3,
                fileName: FileName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_MultipleServiceRecipients()
        {
            string orderDescription = "OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_MultipleServiceRecipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, orderTriage: OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalService: AdditionalServiceName,
                associatedService: AssociatedServiceNameForWebGP,
                multipleServiceRecipients: 3);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Associated Service Only Journey", "Split Order")]
        public void OrderWithAssociatedServiceOnly_MultipleServiceRecipients()
        {
            string orderDescription = "OrderWithAssociatedServiceOnly_MultipleServiceRecipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                associatedService: AssociatedServiceNameForWebGP,
                multipleServiceRecipients: 3);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Associated Service Only Journey", "Split Order")]
        public void OrderAssociatedServiceOnly_EditCatalogueSolution_AddMultipleAssociatedServices()
        {
            string orderDescription = "OrderAssociatedServiceOnly_EditCatalogueSolution_AddMultipleAssociatedServices";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.EditCatalogueSolution(newSolutionName: NewSolutionName, newAdditionalServiceName: string.Empty, newAssociatedServices: new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName });

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Catalogue Solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAdditionalServices()
        {
            string orderDescription = "OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAdditionalServices";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, new List<string> { AdditionalServiceName, NewAdditionalServiceName }, newAssociatedService: string.Empty);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Catalogue Solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAssociatedServices()
        {
            string orderDescription = "OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAssociatedServices";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceName: string.Empty, new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Catalogue Solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAdditional_AddultipleAssociatedServices()
        {
            string orderDescription = "OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAdditional_AddultipleAssociatedServices";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, new List<string> { AdditionalServiceName, NewAdditionalServiceName }, new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Catalogue Solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleServiceRecipients()
        {
            string orderDescription = "OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleServiceRecipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, multipleServiceRecipients: 3);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Catalogue Solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAdditionalServices_MultipleServiceRecipients()
        {
            string orderDescription = "OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAdditionalServices_MultipleServiceRecipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceNames: new List<string> { NewAdditionalServiceName, AdditionalServiceName }, newAssociatedService: string.Empty, multipleServiceRecipients: 3);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Catalogue Solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAssociatedServices_MultipleServiceRecipients()
        {
            string orderDescription = "OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAssociatedServices_MultipleServiceRecipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceNames: null, newAssociatedServices: new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName }, multipleServiceRecipients: 3);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Catalogue Solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAdditionalServices_AddMultipleAssociatedServices_AddMultipleServiceRecipients()
        {
            string orderDescription = "OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAdditionalServices_AddMultipleAssociatedServices_AddMultipleServiceRecipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceNames: new List<string> { NewAdditionalServiceName }, newAssociatedServices: new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName }, multipleServiceRecipients: 3);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Catalogue Solution", "Order")]
        public void OrderWithAssociatedServiceOnly_EditCatalogueSolution_AddMultipleAssociatedServices_AddMultipleServiceRecipients()
        {
            string orderDescription = "OrderWithAssociatedServiceOnly_EditCatalogueSolution_AddMultipleAssociatedServices_AddMultipleServiceRecipients";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: SolutionName,
                associatedService: AssociatedServiceName,
                multipleServiceRecipients: 3);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceName: string.Empty, new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName }, 3);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Catalogue Solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAdditionalServices_AddOneAssociatedService()
        {
            string orderDescription = "OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAdditionalServices_AddOneAssociatedService";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceNames: new List<string> { NewAdditionalServiceName, AdditionalServiceName }, AssociatedServiceNameForWebGP, multipleServiceRecipients: 3);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Additional Service", "Order")]
        public void OrderWithSolutionUnder40K_EditAdditionalService_AddMultipleAdditionalServices()
        {
            string orderDescription = "OrderWithSolutionUnder40K_EditAdditionalService_AddMultipleAdditionalServices";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditAdditionalService(NewSolutionName, new List<string> { NewAdditionalServiceName, AdditionalServiceName });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Additional Service", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalService_MultipleAdditionalServices()
        {
            string orderDescription = "OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalService_MultipleAdditionalServices";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, "Document Management", multipleServiceRecipients: 0);

            OrderingPages.EditAdditionalService(NewSolutionName, new List<string> { NewAdditionalServiceName, AdditionalServiceName }, oldAdditionalService: "Document Management");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Associated Service", "Order")]
        public void OrderWithSolutionUnder40K_EditAssociatedService_AddMultipleAssociatedServices()
        {
            string orderDescription = "OrderWithSolutionUnder40K_EditAssociatedService_AddMultipleAssociatedServices";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditAssociatedService(NewSolutionName, new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName }, string.Empty, string.Empty);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Associated Service", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedService_AddMultipleAssociatedServices()
        {
            string orderDescription = "OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedService_AddMultipleAssociatedServices";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.EditAssociatedService(NewSolutionName, new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName }, oldAssociatedServiceName: "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Associated Service", "Order")]
        public void OrderAssociatedServiceOnly_EditAssociatedService_AddMultipleAssociatedServices()
        {
            string orderDescription = "OrderAssociatedServiceOnly_EditAssociatedService_AddMultipleAssociatedServices";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.AssociatedService);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: NewSolutionName, associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.EditAssociatedServiceOnly(NewSolutionName, new List<string> { NewAssociatedServiceName, AssociatedServiceNameForWebGP }, "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

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

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, competitionName, ServiceRecipientSelectionMode.Single);

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

            CompetitionPages.StepOnePrepareCompetition(FilterType.MultipleResults, competitionName, ServiceRecipientSelectionMode.Single);

            CompetitionPages.StepTwoDefineCompetitionCriteria(CompetitionType.PriceAndNonPriceElement, NonPriceElementType.All);

            CompetitionPages.ViewResults();
        }

        [Fact]
        [Trait("Amend Order Journey", "Order")]
        public void OrderAmendCatalogueSolutionGreaterThan250K()
        {
            string orderDescription = "CatalogueSolutionOver250K";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(EntityFramework.Catalogue.Models.CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Over250K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Over250K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();

            OrderingPages.StepFiveAmendOrder();

            OrderingPages.AmendSolutionsAndServices(NewSolutionName);
        }

        [Fact]
        [Trait("Amend Order Journey", "Order")]
        public void OrderAmendCatalogueSolutionAmendDescription()
        {
            string orderDescription = "Amend_CatalogueSolution";

            string amendOrderDescription = "AmendedOrder_CatalogueSolution";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(CatalogueItemType.Solution);

            OrderingPages.OrderingTriage.SelectOrderTriage(OrderTriageValue.Under40K);

            OrderingPages.StartOrder.ReadyToStart();

            OrderingPages.StepOnePrepareOrder(SupplierName, orderDescription, false, OrderTriageValue.Under40K);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();

            OrderingPages.StepFiveAmendOrder();

            OrderingPages.AmendOrderDescription(amendOrderDescription);

            OrderingPages.AmendSolutionsAndServices(NewSolutionName);
        }
    }
}
