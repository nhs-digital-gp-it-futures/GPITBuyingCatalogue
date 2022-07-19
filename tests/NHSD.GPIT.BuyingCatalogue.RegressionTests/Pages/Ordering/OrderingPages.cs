using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Dashboard;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepOne;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Triage;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;
using OpenQA.Selenium;

namespace NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering
{
    public class OrderingPages
    {
        public OrderingPages(IWebDriver driver, CommonActions commonActions, LocalWebApplicationFactory factory)
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
            AssociatedService = new AssociatedService(driver, commonActions);
            SelectAssociatedServiceRecipents = new SelectAssociatedServiceRecipents(driver, commonActions);
            SelectAndConfirmAssociatedServicePrices = new SelectAndConfirmAssociatedServicePrices(driver, commonActions);
            SelectAdditionalServiceRecipients = new SelectAdditionalServiceRecipients(driver, commonActions);
            SelectAndConfirmAdditionalServicePrice = new SelectAndConfirmAdditionalServicePrice(driver, commonActions);
            Factory = factory;
        }

        internal LocalWebApplicationFactory Factory { get; private set; }

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

        internal AssociatedService AssociatedService { get; }

        internal SelectAssociatedServiceRecipents SelectAssociatedServiceRecipents { get; }

        internal SelectAndConfirmAssociatedServicePrices SelectAndConfirmAssociatedServicePrices { get; }

        internal SelectAdditionalServiceRecipients SelectAdditionalServiceRecipients { get; }

        internal SelectAndConfirmAdditionalServicePrice SelectAndConfirmAdditionalServicePrice { get; }

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

        public void StepTwoAddSolutionsAndServices(string? additionalService = null, string? associatedService = null)
        {
            TaskList.SelectSolutionsAndServicesTask();

            SelectCatalogueSolution.SelectSolution(additionalService);
            SelectCatalogueSolutionServiceRecipients.AddCatalogueSolutionServiceRecipient();
            SelectAndConfirmPrices.SelectAndConfirmPrice();
            Quantity.AddPracticeListSize();

            using var dbContext = Factory.DbContext;

            var hasAdditionalService = dbContext.AdditionalServices.Any(a => a.Solution.CatalogueItem.Name == "Emis Web GP");

            if (hasAdditionalService && !string.IsNullOrWhiteSpace(additionalService))
            {
                SelectAdditionalServiceRecipients.AddServiceRecipients();
                SelectAndConfirmAdditionalServicePrice.SelectAndConfirmPrice();
                Quantity.AddUnitQuantity();
            }

            var hasAssociatedServices = dbContext.SupplierServiceAssociations.Any(ssa => ssa.CatalogueItem.Name == "Emis Web GP");

            if (hasAssociatedServices)
            {
                if (!string.IsNullOrWhiteSpace(associatedService))
                {
                        AssociatedService.AddAssociatedService("Yes");
                        SelectAssociatedServiceRecipents.AddServiceRecipient();
                        SelectAndConfirmAssociatedServicePrices.SelectAndConfirmPrice();
                        Quantity.AddUnitQuantity();
                }
                else
                {
                    AssociatedService.AddAssociatedService();
                }
            }

            SolutionAndServicesReview.ReviewSolutionAndServices();

            TaskList.SelectFundingSourcesTask();
            OrderingStepTwo.AddFundingSources();

            TaskList.ReviewAndCompleteOrderTask();
        }
    }
}
