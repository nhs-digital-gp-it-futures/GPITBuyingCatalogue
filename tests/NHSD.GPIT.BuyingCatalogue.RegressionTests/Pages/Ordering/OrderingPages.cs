using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Dashboard;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepOne;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepThree;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.AssociatedServiceOnly;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.SolutionSelection;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Triage;
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
            StartOrder = new StartOrder(driver, commonActions);
            TaskList = new TaskList(driver, commonActions);
            OrderingStepOne = new OrderingStepOne(driver, commonActions);
            SelectFundingSources = new SelectFundingSources(driver, commonActions);
            SelectSupplier = new SelectSupplier(driver, commonActions);
            SupplierContacts = new SupplierContacts(driver, commonActions);
            SelectCatalogueSolution = new SelectCatalogueSolution(driver, commonActions, factory);
            SelectCatalogueSolutionServiceRecipients = new SelectCatalogueSolutionServiceRecipients(driver, commonActions);
            SelectAndConfirmPrices = new SelectAndConfirmPrices(driver, commonActions);
            Quantity = new Quantity(driver, commonActions);
            SolutionAndServicesReview = new SolutionAndServicesReview(driver, commonActions);
            SelectAssociatedService = new SelectAssociatedService(driver, commonActions);
            SelectAssociatedServiceRecipents = new SelectAssociatedServiceRecipents(driver, commonActions);
            SelectAndConfirmAssociatedServicePrices = new SelectAndConfirmAssociatedServicePrices(driver, commonActions);
            SelectAdditionalServiceRecipients = new SelectAdditionalServiceRecipients(driver, commonActions);
            SelectAndConfirmAdditionalServicePrice = new SelectAndConfirmAdditionalServicePrice(driver, commonActions);
            SelectAssociatedServiceOnly = new SelectAssociatedServiceOnly(driver, commonActions);
            SelectAssociatedServiceRecipientOnly = new SelectAssociatedServiceRecipientOnly(driver, commonActions);
            SelectAndConfirmAssociatedServiceOnlyPrices = new SelectAndConfirmAssociatedServiceOnlyPrices(driver, commonActions);
            OrderingStepThree = new OrderingStepThree(driver, commonActions);
            Factory = factory;
            Driver = driver;
        }

        internal LocalWebApplicationFactory Factory { get; private set; }

        internal OrderingDashboard OrderingDashboard { get; }

        internal OrderType.OrderType OrderType { get; }

        internal OrderingTriage OrderingTriage { get; }

        internal StartOrder StartOrder { get; }

        internal TaskList TaskList { get; }

        internal OrderingStepOne OrderingStepOne { get; }

        internal SelectFundingSources SelectFundingSources { get; }

        internal SelectSupplier SelectSupplier { get; }

        internal SupplierContacts SupplierContacts { get; }

        internal SelectCatalogueSolution SelectCatalogueSolution { get; }

        internal SelectCatalogueSolutionServiceRecipients SelectCatalogueSolutionServiceRecipients { get; }

        internal SelectAndConfirmPrices SelectAndConfirmPrices { get; }

        internal Quantity Quantity { get; }

        internal SolutionAndServicesReview SolutionAndServicesReview { get; }

        internal SelectAssociatedService SelectAssociatedService { get; }

        internal SelectAssociatedServiceRecipents SelectAssociatedServiceRecipents { get; }

        internal SelectAndConfirmAssociatedServicePrices SelectAndConfirmAssociatedServicePrices { get; }

        internal SelectAdditionalServiceRecipients SelectAdditionalServiceRecipients { get; }

        internal SelectAndConfirmAdditionalServicePrice SelectAndConfirmAdditionalServicePrice { get; }

        internal SelectAssociatedServiceOnly SelectAssociatedServiceOnly { get; }

        internal SelectAssociatedServiceRecipientOnly SelectAssociatedServiceRecipientOnly { get; }

        internal SelectAndConfirmAssociatedServiceOnlyPrices SelectAndConfirmAssociatedServiceOnlyPrices { get; }

        internal OrderingStepThree OrderingStepThree { get; }

        internal IWebDriver Driver { get; }

        public void StepOnePrepareOrder(
            string supplierName,
            bool addNewSupplierContact = false,
            EntityFramework.Ordering.Models.OrderTriageValue orderTriage = EntityFramework.Ordering.Models.OrderTriageValue.Under40K,
            EntityFramework.Catalogue.Models.CatalogueItemType itemType = EntityFramework.Catalogue.Models.CatalogueItemType.Solution)
        {
            TaskList.OrderDescriptionTask();
            OrderingStepOne.AddOrderDescription();

            TaskList.CallOffOrderingPartyContactDetailsTask();
            OrderingStepOne.AddCallOffOrderingPartyContactDetails();

            TaskList.SupplierInformationAndContactDetailsTask();
            SelectSupplier.SelectAndConfirmSupplier(supplierName);
            SupplierContacts.ConfirmContact(addNewSupplierContact);

            TaskList.TimescalesForCallOffAgreementTask();
            OrderingStepOne.AddTimescaleForCallOffAgreement(orderTriage, itemType);
        }

        public void StepTwoAddSolutionsAndServices(string solutionName, string additionalService = "", string associatedService = "")
        {
            using var dbContext = Factory.DbContext;

            var hasAdditionalService = dbContext.AdditionalServices.Any(a => a.Solution.CatalogueItem.Name == solutionName);

            var hasAssociatedServices = dbContext.SupplierServiceAssociations.Any(ssa => ssa.CatalogueItem.Name == solutionName);

            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnly();

            TaskList.SelectSolutionsAndServicesTask(isAssociatedServiceOnlyOrder);

            if (!isAssociatedServiceOnlyOrder)
            {
                SelectCatalogueSolution.SelectSolution(solutionName, additionalService);

                SelectCatalogueSolutionServiceRecipients.AddCatalogueSolutionServiceRecipient();
                SelectAndConfirmPrices.SelectAndConfirmPrice();
                Quantity.AddQuantity();

                if (hasAdditionalService && !string.IsNullOrWhiteSpace(additionalService))
                {
                    SelectAdditionalServiceRecipients.AddServiceRecipients();
                    SelectAndConfirmAdditionalServicePrice.SelectAndConfirmPrice();
                    Quantity.AddQuantity();
                }

                if (hasAssociatedServices)
                {
                    if (!string.IsNullOrWhiteSpace(associatedService))
                    {
                        SelectAssociatedService.AddAssociatedService("Yes", associatedService);
                        SelectAssociatedServiceRecipents.AddServiceRecipient();
                        SelectAndConfirmAssociatedServicePrices.SelectAndConfirmPrice();
                        Quantity.AddQuantity();
                    }
                    else
                    {
                        SelectAssociatedService.AddAssociatedService();
                    }
                }
            }
            else
            {
                SelectAssociatedServiceOnly.SelectAssociatedServices(solutionName, associatedService);
                SelectAssociatedServiceRecipientOnly.AddServiceRecipient();
                SelectAndConfirmAssociatedServiceOnlyPrices.SelectAndConfirmPrice();
                Quantity.AddQuantity();
            }

            SolutionAndServicesReview.ReviewSolutionAndServices();

            TaskList.SelectFundingSourcesTask();
            SelectFundingSources.AddFundingSources(solutionName, additionalService, associatedService, isAssociatedServiceOnlyOrder);
        }

        public void StepThreeReviewAndCompleteOrder()
        {
            TaskList.ReviewAndCompleteOrderTask();
            OrderingStepThree.ReviewAndCompleteOrder();
        }

        public void EditSolutionAndServices()
        {
            TaskList.SelectSolutionsAndServicesTask(IsAssociatedServiceOnly());
        }

        private bool IsAssociatedServiceOnly()
        {
            using var dbContext = Factory.DbContext;
            var orderID = Driver.Url.Split('/').Last().Split('-')[0].Replace("C0", string.Empty);

            return dbContext.Orders.Any(o => string.Equals(o.Id.ToString(), orderID) && o.AssociatedServicesOnly);
        }
    }
}
