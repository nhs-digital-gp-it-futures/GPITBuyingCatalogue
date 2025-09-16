using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.OrderType;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using Xunit;
using Xunit.Abstractions;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Scenarios
{
    public class OrderScenarios(LocalWebApplicationFactory factory, ITestOutputHelper testOutputHelper)
        : BuyerTestBase(factory, testOutputHelper), IClassFixture<LocalWebApplicationFactory>
    {
        private const string FileName = "valid_service_recipients.csv";
        private const string SupplierName = "EMIS Health";
        private const string SolutionName = "Anywhere Consult";
        private const string SolutionForLocalfundingonly = "Online and Video Consult";
        private const string AssociatedServiceName = "Anywhere Consult – Integrated Device";
        private const string AssociatedServiceMerger = "Practice Merge";
        private const string AssociatedServiceSplit = "Practice Reorganisation";
        private const string AdditionalServiceName = "Automated Arrivals";
        private const string NewSolutionName = "Emis Web GP";
        private const string NewAdditionalServiceName = "EMIS Mobile";
        private const string NewAssociatedServiceName = "Automated Arrivals – Specialist Cabling";
        private const string AssociatedServiceNameForWebGP = "Installation";

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionUnder40K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Framework", "Order")]
        public void LocalFundingOnlyFrameworksOrderWithSolutionUnder40K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.DFOCVC);

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionForLocalfundingonly);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals", associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void EditPlannedDeliveryDateOrderWithSolutionAdditionalAndAssociatedServiceUnder40K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals", associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.EditPlannedDeliveryDate("Emis Web GP", additionalService: "Automated Arrivals", associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void CatalogueSolutionOnlyBetween40KTo250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();

            OrderingPages.StepFiveAmendOrder();
        }

        [Fact]
        [Trait("Amend Order Journey", "Order")]
        public void Amend_CatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

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
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

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
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

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
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

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
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

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
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName, true);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServiceBetween40Kand250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServiceBetween40Kand250KStepThreeCustomRoute()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAssociatedServiceBetween40Kand250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceBetween40Kand250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals", associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeCompleteContract(false);

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceBetween40Kand250KStepThreeCustomRoute()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals", associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeCompleteContract(false);

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void CatalogueSolutionOnlyOver250K_AllserviceRecipients()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName, allServiceRecipients: true);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void CatalogueSolutionOnlyOver250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void CatalogueSolutionOnlyOver250KStepThreeCustomRoute()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.StepThreeCompleteContract(false);

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAssociatedServiceOver250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServiceOver250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceOver250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices("Emis Web GP", additionalService: "Automated Arrivals", associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Associated Service Only Journey", "Order")]
        public void OrderAssociatedServiceOnly()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.Tech_Innovation, CatalogueItemType.AssociatedService);

            OrderingPages.StepOnePrepareOrder(SupplierName, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Associated Service Only Journey", "Split Order")]
        public void OrderSplitAssociatedServiceOnly()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.Tech_Innovation, CatalogueItemType.AssociatedService, AssociatedServiceType.AssociatedServiceSplit);

            OrderingPages.StepOnePrepareOrder(SupplierName, itemType: CatalogueItemType.AssociatedService, associatedServiceType: AssociatedServiceType.AssociatedServiceSplit);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceSplit, multipleServiceRecipients: 3);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Associated Service Only Journey", "Merger Order")]
        public void OrderMergerAssociatedServiceOnly()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.Tech_Innovation, CatalogueItemType.AssociatedService, AssociatedServiceType.AssociatedServiceMerger);

            OrderingPages.StepOnePrepareOrder(SupplierName, itemType: CatalogueItemType.AssociatedService, associatedServiceType: AssociatedServiceType.AssociatedServiceMerger);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceMerger, multipleServiceRecipients: 3);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Associated Service Only Journey", "Order")]
        public void EditPlannedDeliveryDateOrderAssociatedServiceOnly()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.Tech_Innovation, CatalogueItemType.AssociatedService);

            OrderingPages.StepOnePrepareOrder(SupplierName, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.EditPlannedDeliveryDate("Anywhere Consult", "Anywhere Consult – Integrated Device", " ");

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Associated Service Only Journey", "Order")]
        public void OrderAssociatedServiceOnlyWithStepThreeCustomRoute()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.Tech_Innovation, CatalogueItemType.AssociatedService);

            OrderingPages.StepOnePrepareOrder(SupplierName, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.StepThreeContractAssociatedServices(false);

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit catalogue solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit catalogue solution", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit catalogue solution", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(newSolutionName: NewSolutionName, newAssociatedService: NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit catalogue solution", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(newSolutionName: NewSolutionName, newAdditionalServiceName: NewAdditionalServiceName, newAssociatedService: NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit catalogue solution", "Order")]
        public void OrderAssociatedServiceOnly_EditCatalogueSolution()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.Tech_Innovation, CatalogueItemType.AssociatedService);

            OrderingPages.StepOnePrepareOrder(SupplierName, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.EditCatalogueSolution(newSolutionName: NewSolutionName, newAssociatedService: NewAssociatedServiceName);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit additional service", "Order")]
        public void OrderWithSolutionUnder40K_SolutionDoesNotHaveAdditionalService_EditAdditionalService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditAdditionalService(SolutionName, NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit additional service", "Order")]
        public void OrderWithSolutionUnder40K_EditAdditionalService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditAdditionalService(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit additional service", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, AdditionalServiceName);

            OrderingPages.EditAdditionalService(NewSolutionName, NewAdditionalServiceName, oldAdditionalService: AdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit additional service", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditAdditionalService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditAdditionalService(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit additional service", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_EditAdditionalService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, AdditionalServiceName, NewAssociatedServiceName);

            OrderingPages.EditAdditionalService(NewSolutionName, NewAdditionalServiceName, oldAdditionalService: AdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit associated service", "Order")]
        public void OrderWithSolutionUnder40K_EditAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditAssociatedService(NewSolutionName, NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit associated service", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.EditAssociatedService(NewSolutionName, NewAssociatedServiceName, NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit associated service", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, associatedService: AssociatedServiceNameForWebGP);

            OrderingPages.EditAssociatedService(NewSolutionName, NewAssociatedServiceName, oldAssociatedServiceName: AssociatedServiceNameForWebGP);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit associated service", "Order")]
        public void OrderWithSolutionAdditionalAndAssociatedServiceUnder40K_EditAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, NewAdditionalServiceName, AssociatedServiceNameForWebGP);

            OrderingPages.EditAssociatedService(NewSolutionName, NewAssociatedServiceName, NewAdditionalServiceName, AssociatedServiceNameForWebGP);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Associated Service Only Journey", "Order")]
        public void OrderAssociatedServiceOnly_EditAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.Tech_Innovation, CatalogueItemType.AssociatedService);

            OrderingPages.StepOnePrepareOrder(SupplierName, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: NewSolutionName, associatedService: AssociatedServiceNameForWebGP);

            OrderingPages.EditAssociatedServiceOnly(NewSolutionName, NewAssociatedServiceName, AssociatedServiceNameForWebGP);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolutionServiceRecipient()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditCatalogueSolutionServiceRecipient(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolutionPrice()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditCatalogueSolutionPrice(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolutionQuantity()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditCatalogueItemQuantity(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalServiceRecipient()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.EditAdditionalServiceRecipient(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalServicePrice()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.EditAdditionalServicePrice(NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalServiceQuantity()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, NewAdditionalServiceName);

            OrderingPages.EditCatalogueItemQuantity(NewAdditionalServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedServiceRecipient()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditAssociatedServiceRecipient(NewSolutionName, NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedServicePrice()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditAssociatedServicePrice(NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedServiceQuantity()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditCatalogueItemQuantity(NewAssociatedServiceName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderAssociatedServiceOnly_EditAssociatedServiceRecipients()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.Tech_Innovation, CatalogueItemType.AssociatedService);

            OrderingPages.StepOnePrepareOrder(SupplierName, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditAssociatedServiceOnlyServiceRecipients(NewAssociatedServiceName);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderAssociatedServiceOnly_EditAssociatedServicePrice()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.Tech_Innovation, CatalogueItemType.AssociatedService);

            OrderingPages.StepOnePrepareOrder(SupplierName, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditAssociatedServiceOnlyPrice(NewAssociatedServiceName);

            OrderingPages.StepThreeContractAssociatedServices(false);

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Order Journey", "Order")]
        public void OrderAssociatedServiceOnly_EditAssociatedServiceQuantity()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.Tech_Innovation, CatalogueItemType.AssociatedService);

            OrderingPages.StepOnePrepareOrder(SupplierName, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: NewSolutionName, associatedService: NewAssociatedServiceName);

            OrderingPages.EditCatalogueItemQuantity(NewAssociatedServiceName);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionAndAdditionalServicesUnder40K_MultipleAdditionalServices()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

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
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

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
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

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
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

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
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

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
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.Tech_Innovation, CatalogueItemType.AssociatedService);

            OrderingPages.StepOnePrepareOrder(SupplierName, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalServices: null,
                associatedServices: new List<string> { NewAssociatedServiceName, AssociatedServiceNameForWebGP });

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Order Journey", "Order")]
        public void OrderWithSolutionUnder40K_MultipleServiceRecipients()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

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
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

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
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

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
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalService: AdditionalServiceName,
                importServiceRecipients: true,
                fileName: FileName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Associated Service Only Journey", "Order")]
        public void OrderAssociatedServiceOnly_ImportServiceRecipients()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.Tech_Innovation, CatalogueItemType.AssociatedService);

            OrderingPages.StepOnePrepareOrder(SupplierName, itemType: CatalogueItemType.AssociatedService);

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
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

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
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

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
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                additionalService: AdditionalServiceName,
                associatedService: AssociatedServiceNameForWebGP,
                multipleServiceRecipients: 3);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Associated Service Only Journey", "Order")]
        public void OrderWithAssociatedServiceOnly_MultipleServiceRecipients()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.Tech_Innovation, CatalogueItemType.AssociatedService);

            OrderingPages.StepOnePrepareOrder(SupplierName, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: NewSolutionName,
                associatedService: AssociatedServiceNameForWebGP,
                multipleServiceRecipients: 3);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Associated Service Only Journey", "Order")]
        public void OrderAssociatedServiceOnly_EditCatalogueSolution_AddMultipleAssociatedServices()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.Tech_Innovation, CatalogueItemType.AssociatedService);

            OrderingPages.StepOnePrepareOrder(SupplierName, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: SolutionName, associatedService: AssociatedServiceName);

            OrderingPages.EditCatalogueSolution(newSolutionName: NewSolutionName, newAdditionalServiceName: string.Empty, newAssociatedServices: new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName });

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit catalogue solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAdditionalServices()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, new List<string> { AdditionalServiceName, NewAdditionalServiceName }, newAssociatedService: string.Empty);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit catalogue solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAssociatedServices()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceName: string.Empty, new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit catalogue solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAdditional_AddultipleAssociatedServices()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, new List<string> { AdditionalServiceName, NewAdditionalServiceName }, new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit catalogue solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleServiceRecipients()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, multipleServiceRecipients: 3);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit catalogue solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAdditionalServices_MultipleServiceRecipients()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceNames: new List<string> { NewAdditionalServiceName, AdditionalServiceName }, newAssociatedService: string.Empty, multipleServiceRecipients: 3);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit catalogue solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAssociatedServices_MultipleServiceRecipients()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceNames: null, newAssociatedServices: new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName }, multipleServiceRecipients: 3);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit catalogue solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAdditionalServices_AddMultipleAssociatedServices_AddMultipleServiceRecipients()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceNames: new List<string> { NewAdditionalServiceName }, newAssociatedServices: new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName }, multipleServiceRecipients: 3);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit catalogue solution", "Order")]
        public void OrderWithAssociatedServiceOnly_EditCatalogueSolution_AddMultipleAssociatedServices_AddMultipleServiceRecipients()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.Tech_Innovation, CatalogueItemType.AssociatedService);

            OrderingPages.StepOnePrepareOrder(SupplierName, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(
                solutionName: SolutionName,
                associatedService: AssociatedServiceName,
                multipleServiceRecipients: 3);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceName: string.Empty, new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName }, 3);

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit catalogue solution", "Order")]
        public void OrderWithSolutionUnder40K_EditCatalogueSolution_AddMultipleAdditionalServices_AddOneAssociatedService()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(SolutionName);

            OrderingPages.EditCatalogueSolution(NewSolutionName, newAdditionalServiceNames: new List<string> { NewAdditionalServiceName, AdditionalServiceName }, AssociatedServiceNameForWebGP, multipleServiceRecipients: 3);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit additional service", "Order")]
        public void OrderWithSolutionUnder40K_EditAdditionalService_AddMultipleAdditionalServices()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditAdditionalService(NewSolutionName, new List<string> { NewAdditionalServiceName, AdditionalServiceName });

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit additional service", "Order")]
        public void OrderWithSolutionAndAdditionalServiceUnder40K_EditAdditionalService_MultipleAdditionalServices()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, "Document Management", multipleServiceRecipients: 0);

            OrderingPages.EditAdditionalService(NewSolutionName, new List<string> { NewAdditionalServiceName, AdditionalServiceName }, oldAdditionalService: "Document Management");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit associated service", "Order")]
        public void OrderWithSolutionUnder40K_EditAssociatedService_AddMultipleAssociatedServices()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.EditAssociatedService(NewSolutionName, new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName }, string.Empty, string.Empty);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit associated service", "Order")]
        public void OrderWithSolutionAndAssociatedServiceUnder40K_EditAssociatedService_AddMultipleAssociatedServices()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName, associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.EditAssociatedService(NewSolutionName, new List<string> { AssociatedServiceNameForWebGP, NewAssociatedServiceName }, oldAssociatedServiceName: "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Edit Associated Service Only Journey", "Order")]
        public void OrderAssociatedServiceOnly_EditAssociatedService_AddMultipleAssociatedServices()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType(FrameworkType.Tech_Innovation, CatalogueItemType.AssociatedService);

            OrderingPages.StepOnePrepareOrder(SupplierName, itemType: CatalogueItemType.AssociatedService);

            OrderingPages.StepTwoAddSolutionsAndServices(solutionName: NewSolutionName, associatedService: "Automated Arrivals – Engineering Half Day");

            OrderingPages.EditAssociatedServiceOnly(NewSolutionName, new List<string> { NewAssociatedServiceName, AssociatedServiceNameForWebGP }, "Automated Arrivals – Engineering Half Day");

            OrderingPages.StepThreeContractAssociatedServices();

            OrderingPages.StepFourReviewAndCompleteOrder();
        }

        [Fact]
        [Trait("Amend Order Journey", "Order")]
        public void OrderAmendCatalogueSolutionGreaterThan250K()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

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
            string amendOrderDescription = "AmendedOrder_CatalogueSolution";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();

            OrderingPages.StepFiveAmendOrder();

            OrderingPages.AmendOrderDescription(amendOrderDescription);

            OrderingPages.AmendSolutionsAndServices(NewSolutionName);
        }

        [Fact]
        [Trait("Summary screen minor amendment", "Change description")]
        public void OrderNonAmendmentChangeOrderDescription()
        {
            string amendOrderDescription = "Order with description changed via the summary screen";

            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();

            OrderingPages.SummaryScreenChangeDescription(amendOrderDescription);
        }

        [Fact]
        [Trait("Summary screen minor amendment", "Change ordering party contact")]
        public void OrderNonAmendmentChangeOrderOrderingPartyContact()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();

            OrderingPages.SummaryScreenChangeOrderingPartyContact();
        }

        [Fact]
        [Trait("Summary screen minor amendment", "Change supplier contact")]
        public void OrderNonAmendmentChangeOrderSupplierContact()
        {
            OrderingPages.OrderingDashboard.CreateNewOrder();

            OrderingPages.OrderType.ChooseOrderType();

            OrderingPages.StepOnePrepareOrder(SupplierName);

            OrderingPages.StepTwoAddSolutionsAndServices(NewSolutionName);

            OrderingPages.StepThreeCompleteContract();

            OrderingPages.StepFourReviewAndCompleteOrder();

            OrderingPages.SummaryScreenChangeSupplierContact();
        }
    }
}
