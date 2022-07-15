using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Dashboard;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepOne;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Triage;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering
{
    public class OrderingPages
    {
        public OrderingPages(IWebDriver driver, CommonActions commonActions)
        {
            OrderingDashboard = new OrderingDashboard(driver, commonActions);
            OrderType = new OrderType.OrderType(driver, commonActions);
            OrderingTriage = new OrderingTriage(driver, commonActions);
            TaskList = new TaskList(driver, commonActions);
            OrderingStepOne = new OrderingStepOne(driver, commonActions);
            OrderingStepTwo = new OrderingStepTwo(driver, commonActions);
            SelectSupplier = new SelectSupplier(driver, commonActions);
            SupplierContact = new SupplierContact(driver, commonActions);
            SelectCatalogueSolution = new SelectCatalogueSolution(driver, commonActions);
            SelectCatalogueSolutionServiceRecipients = new SelectCatalogueSolutionServiceRecipients(driver, commonActions);
            SelectAndConfirmPrices = new SelectAndConfirmPrices(driver, commonActions);
            Quantity = new Quantity(driver, commonActions);
            SolutionAndServicesReview = new SolutionAndServicesReview(driver, commonActions);
        }

        internal OrderingDashboard OrderingDashboard { get; }

        internal OrderType.OrderType OrderType { get; }

        internal OrderingTriage OrderingTriage { get; }

        internal TaskList TaskList { get; }

        internal OrderingStepOne OrderingStepOne { get; }

        internal OrderingStepTwo OrderingStepTwo { get; }

        internal SelectSupplier SelectSupplier { get; }

        internal SupplierContact SupplierContact { get; }

        internal SelectCatalogueSolution SelectCatalogueSolution { get; }

        internal SelectCatalogueSolutionServiceRecipients SelectCatalogueSolutionServiceRecipients { get; }

        internal SelectAndConfirmPrices SelectAndConfirmPrices { get; }

        internal Quantity Quantity { get; }

        internal SolutionAndServicesReview SolutionAndServicesReview { get; }

        public void StepOnePrepareOrder(bool addNewSupplierContact = false)
        {
            TaskList.OrderDescriptionTask();
            OrderingStepOne.AddOrderDescription();

            TaskList.CallOffOrderingPartyContactDetailsTask();
            OrderingStepOne.AddCallOffOrderingPartyContactDetails();

            TaskList.SupplierInformationAndContactDetailsTask();
            SelectSupplier.SelectAndConfirmSupplier();
            SupplierContact.ConfirmContact(addNewSupplierContact);

            TaskList.TimescalesForCallOffAgreementTask();
            OrderingStepOne.AddTimescaleForCallOffAgreement();
        }

        public void StepTwoAddSolutionsAndServices(bool withAdditionalService = false)
        {
            TaskList.SelectSolutionsAndServicesTask();

            SelectCatalogueSolution.SelectSolution(withAdditionalService);
            SelectCatalogueSolutionServiceRecipients.AddCatalogueSolutionServiceRecipient();
            SelectAndConfirmPrices.SelectAndConfirmPrice();
            Quantity.AddQuantity();

            if (SelectCatalogueSolution.ProvidesAssociatedService())
            {
                // if provide assoc service navigates to a page to select if we want an assoc or not
            }

            SolutionAndServicesReview.ReviewSolutionAndServices();

            TaskList.SelectFundingSourcesTask();
            OrderingStepTwo.AddFundingSources();

            TaskList.ReviewAndCompleteOrderTask();
        }
    }
}
