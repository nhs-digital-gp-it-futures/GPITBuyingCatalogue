using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.Contracts;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Objects.Ordering.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Utils;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.Contracts;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.FundingSource;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Dashboard
{
    public sealed class TaskList : PageBase
    {
        public TaskList(IWebDriver driver, CommonActions commonActions)
            : base(driver, commonActions)
        {
        }

        public void OrderDescriptionTask()
        {
            CommonActions.ClickLinkElement(OrderDashboard.OrderDescriptionLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderDescriptionController),
                nameof(OrderDescriptionController.NewOrderDescription))
                    .Should().BeTrue();
        }

        public void AmendOrderDescriptionTask(string amendOrderDescription)
        {
            CommonActions.ClickLinkElement(OrderDashboard.OrderDescriptionLink);
            CommonActions.ClearInputElement(OrderDescription.DescriptionInput);
            CommonActions.LedeText().Should().Be("You can update the description of this order to help distinguish it from the previous version if required.".FormatForComparison());
        }

        public void CallOffOrderingPartyContactDetailsTask()
        {
            CommonActions.ClickLinkElement(CalloffPartyInformation.CallOffOrderingPartyContactDetailsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(OrderingPartyController),
                nameof(OrderingPartyController.OrderingParty))
                    .Should().BeTrue();
        }

        public void SupplierInformationAndContactDetailsTask()
        {
            CommonActions.ClickLinkElement(SupplierObjects.SupplierContactDetailsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(SupplierController),
                nameof(SupplierController.SelectSupplier))
                    .Should().BeTrue();
        }

        public void TimescalesForCallOffAgreementTask()
        {
            CommonActions.ClickLinkElement(CommencementDateObjects.TimescalesForCallOffLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(CommencementDateController),
                nameof(CommencementDateController.CommencementDate))
                    .Should().BeTrue();
        }

        public void SelectOrderRecipients()
        {
            CommonActions.ClickLinkElement(OrderRecipientsObjects.ServiceRecipientsLink);

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(ServiceRecipientsController),
               nameof(ServiceRecipientsController.SelectServiceRecipients))
                   .Should().BeTrue();
        }

        public void SelectSolutionsAndServicesTask(bool isAssociatedServiceOnly)
        {
            CommonActions.ClickLinkElement(CatalogueSolutionObjects.SelectSolutionsAndServicesLink);

            if (isAssociatedServiceOnly)
            {
                CommonActions.PageLoadedCorrectGetIndex(
                   typeof(CatalogueSolutionsController),
                   nameof(CatalogueSolutionsController.SelectSolutionAssociatedServicesOnly))
                       .Should().BeTrue();
            }
            else
            {
                CommonActions.PageLoadedCorrectGetIndex(
                    typeof(CatalogueSolutionsController),
                    nameof(CatalogueSolutionsController.SelectSolution))
                        .Should().BeTrue();
            }
        }

        public void AmendSolutionAndServicesTask()
        {
            CommonActions.ClickLinkElement(CatalogueSolutionObjects.SelectSolutionsAndServicesLink);

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(TaskListController),
             nameof(TaskListController.TaskList)).Should().BeTrue();
        }

        public void EditCatalogueAdditionalAndAssociatedServiceTask()
        {
            CommonActions.PageLoadedCorrectGetIndex(
            typeof(TaskListController),
            nameof(TaskListController.TaskList)).Should().BeTrue();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(ReviewSolutionsController),
             nameof(ReviewSolutionsController.ReviewSolutions)).Should().BeTrue();

            CommonActions.ClickContinue();

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(OrderController),
             nameof(OrderController.Order)).Should().BeTrue();
        }

        public void EditSolutionsAndServicesTask(bool isAssociatedServiceOnly)
        {
            CommonActions.ClickLinkElement(CatalogueSolutionObjects.SelectSolutionsAndServicesLink);

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(ReviewSolutionsController),
             nameof(ReviewSolutionsController.ReviewSolutions)).Should().BeTrue();

            if (isAssociatedServiceOnly)
            {
                CommonActions.ClickLinkElement(ReviewSolutionsObjects.EditAssociatedServicesLink);
            }
            else
            {
                CommonActions.ClickLinkElement(ReviewSolutionsObjects.EditSolutionAndServicesLink);
            }

            CommonActions.PageLoadedCorrectGetIndex(
             typeof(TaskListController),
             nameof(TaskListController.TaskList)).Should().BeTrue();
        }

        public void SelectFrameWork()
        {
            CommonActions.ClickLinkElement(FundingSources.SelectFundingSourcesLink);
            CommonActions.PageLoadedCorrectGetIndex(
                    typeof(FundingSourceController),
                    nameof(FundingSourceController.SelectFramework))
                .Should()
                .BeTrue();

            CommonActions.ClickLastRadio();
            CommonActions.ClickSave();
        }

        public void SelectPlannedDeliveryDatesTask()
        {
            CommonActions.ClickLinkElement(OrderDashboard.PlannedDeliveryDates);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.SelectDate)).Should().BeTrue();
        }

        public void EditPlannedDeliveryDateTask()
        {
            CommonActions.ClickLinkElement(OrderDashboard.PlannedDeliveryDates);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DeliveryDatesController),
                nameof(DeliveryDatesController.Review)).Should().BeTrue();
        }

        public void SelectFundingSourcesTask()
        {
            CommonActions.ClickLinkElement(FundingSources.SelectFundingSourcesLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FundingSourceController),
                nameof(FundingSourceController.FundingSources))
                    .Should().BeTrue();
        }

        public void SelectLocalFundingSourcesTask()
        {
            CommonActions.ClickLinkElement(FundingSources.SelectFundingSourcesLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(FundingSourceController),
                nameof(FundingSourceController.FundingSources))
                    .Should().BeTrue();

            CommonActions.ClickSave();

            CommonActions.PageLoadedCorrectGetIndex(
              typeof(OrderController),
              nameof(OrderController.Order)).Should().BeTrue();
        }

        public void ImplementationPlanMilestonesTask()
        {
            CommonActions.ClickLinkElement(ImplementationPlanObjects.ImplementationMilestonesAndPaymentTriggers);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ImplementationPlanController),
                nameof(ImplementationPlanController.Index))
                    .Should().BeTrue();
        }

        public void AssociatedServiceBillingAndRequirementsTask()
        {
            CommonActions.ClickLinkElement(AssociatedServicesBillingObjects.AssociatedServiceBillingAndRequirementsLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(ContractBillingController),
                nameof(ContractBillingController.Index))
                    .Should().BeTrue();
        }

        public void DataProcessingInformationTask()
        {
            CommonActions.ClickLinkElement(DataProcessingPlanObjects.DataProcessingInformationLink);

            CommonActions.PageLoadedCorrectGetIndex(
                typeof(DataProcessingPlanController),
                nameof(DataProcessingPlanController.Index))
                    .Should().BeTrue();

            CommonActions.ClickSave();
        }

        public void ReviewAndCompleteOrderTask()
        {
            CommonActions.ClickLinkElement(OrderSummaryObjects.ReviewAndCompleteOrderLink);

            CommonActions.PageLoadedCorrectGetIndex(
               typeof(OrderController),
               nameof(OrderController.Summary))
                   .Should().BeTrue();
        }
    }
}
