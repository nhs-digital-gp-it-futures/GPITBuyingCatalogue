using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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
            SelectEditCatalogueSolution = new SelectEditCatalogueSolution(driver, commonActions, factory);
            SelectCatalogueSolutionServiceRecipients = new SelectCatalogueSolutionServiceRecipients(driver, commonActions);
            SelectAndConfirmPrices = new SelectAndConfirmPrices(driver, commonActions);
            Quantity = new Quantity(driver, commonActions);
            SolutionAndServicesReview = new SolutionAndServicesReview(driver, commonActions);
            SelectEditAssociatedService = new SelectEditAssociatedService(driver, commonActions, factory);
            SelectAssociatedServiceRecipents = new SelectAssociatedServiceRecipents(driver, commonActions);
            SelectAndConfirmAssociatedServicePrices = new SelectAndConfirmAssociatedServicePrices(driver, commonActions);
            SelectAdditionalServiceRecipients = new SelectAdditionalServiceRecipients(driver, commonActions);
            SelectAndConfirmAdditionalServicePrice = new SelectAndConfirmAdditionalServicePrice(driver, commonActions);
            SelectEditAssociatedServiceOnly = new SelectEditAssociatedServiceOnly(driver, commonActions);
            SelectAssociatedServiceRecipientOnly = new SelectAssociatedServiceRecipientOnly(driver, commonActions);
            SelectAndConfirmAssociatedServiceOnlyPrices = new SelectAndConfirmAssociatedServiceOnlyPrices(driver, commonActions);
            OrderingStepThree = new OrderingStepThree(driver, commonActions);
            OrderingStepFour = new OrderingStepFour(driver, commonActions);
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

        internal SelectEditCatalogueSolution SelectEditCatalogueSolution { get; }

        internal SelectCatalogueSolutionServiceRecipients SelectCatalogueSolutionServiceRecipients { get; }

        internal SelectAndConfirmPrices SelectAndConfirmPrices { get; }

        internal Quantity Quantity { get; }

        internal SolutionAndServicesReview SolutionAndServicesReview { get; }

        internal SelectEditAssociatedService SelectEditAssociatedService { get; }

        internal SelectAssociatedServiceRecipents SelectAssociatedServiceRecipents { get; }

        internal SelectAndConfirmAssociatedServicePrices SelectAndConfirmAssociatedServicePrices { get; }

        internal SelectAdditionalServiceRecipients SelectAdditionalServiceRecipients { get; }

        internal SelectAndConfirmAdditionalServicePrice SelectAndConfirmAdditionalServicePrice { get; }

        internal SelectEditAssociatedServiceOnly SelectEditAssociatedServiceOnly { get; }

        internal SelectAssociatedServiceRecipientOnly SelectAssociatedServiceRecipientOnly { get; }

        internal SelectAndConfirmAssociatedServiceOnlyPrices SelectAndConfirmAssociatedServiceOnlyPrices { get; }

        internal OrderingStepThree OrderingStepThree { get; }

        internal OrderingStepFour OrderingStepFour { get; }

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
            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnly();

            TaskList.SelectSolutionsAndServicesTask(isAssociatedServiceOnlyOrder);

            if (!isAssociatedServiceOnlyOrder)
            {
                SelectEditCatalogueSolution.SelectSolution(solutionName, additionalService);

                SelectCatalogueSolutionServiceRecipients.AddCatalogueSolutionServiceRecipient();
                SelectAndConfirmPrices.SelectAndConfirmPrice();
                Quantity.AddQuantity();

                if (HasAdditionalService(solutionName) && !string.IsNullOrWhiteSpace(additionalService))
                {
                    SelectAdditionalServiceRecipients.AddServiceRecipients();
                    SelectAndConfirmAdditionalServicePrice.SelectAndConfirmPrice();
                    Quantity.AddQuantity();
                }

                if (HasAssociatedServices(solutionName))
                {
                    if (!string.IsNullOrWhiteSpace(associatedService))
                    {
                        SelectEditAssociatedService.AddAssociatedService("Yes", associatedService);
                        SelectAssociatedServiceRecipents.AddServiceRecipient();
                        SelectAndConfirmAssociatedServicePrices.SelectAndConfirmPrice();
                        Quantity.AddQuantity();
                    }
                    else
                    {
                        SelectEditAssociatedService.AddAssociatedService();
                    }
                }
            }
            else
            {
                SelectEditAssociatedServiceOnly.SelectAssociatedServices(solutionName, associatedService);
                SelectAssociatedServiceRecipientOnly.AddServiceRecipient();
                SelectAndConfirmAssociatedServiceOnlyPrices.SelectAndConfirmPrice();
                Quantity.AddQuantity();
            }

            SolutionAndServicesReview.ReviewSolutionAndServices();

            TaskList.SelectFundingSourcesTask();
            SelectFundingSources.AddFundingSources(solutionName, additionalService, associatedService, isAssociatedServiceOnlyOrder);
        }

        public void StepThreeCompleteContract()
        {
            TaskList.ImplementationPlanMilestonesTask();
            OrderingStepThree.SelectImplementationPlan();

            using var dbContext = Factory.DbContext;
            var orderID = Driver.Url.Split('/').Last().Split('-')[0].Replace("C0", string.Empty);

            var isOrderWithAssociatedService = dbContext.Orders
                .Any(o => string.Equals(o.Id.ToString(), orderID) &&
                o.OrderItems.Any(i => i.CatalogueItem.CatalogueItemType == EntityFramework.Catalogue.Models.CatalogueItemType.AssociatedService));

            if (isOrderWithAssociatedService)
            {
                TaskList.AssociatedServiceBillingAndRequirementsTask();
                OrderingStepThree.SelectAssociatedServicesBilling();
            }

            TaskList.DataProcessingInformationTask();
            OrderingStepThree.SelectPersonalDataProcessingInformation();
        }

        public void StepFourReviewAndCompleteOrder()
        {
            TaskList.ReviewAndCompleteOrderTask();
            OrderingStepFour.ReviewAndCompleteOrder();
        }

        public void EditCatalogueSolution(string newSolutionName, string newAdditionalServiceName = "", string newAssociatedService = "")
        {
            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnly();

            TaskList.EditSolutionsAndServicesTask(isAssociatedServiceOnlyOrder);

            if (isAssociatedServiceOnlyOrder)
            {
                SelectEditAssociatedServiceOnly.EditSolutionForAssociatedService(newSolutionName, newAssociatedService);
                SelectAssociatedServiceRecipientOnly.AddServiceRecipient();
                SelectAndConfirmAssociatedServiceOnlyPrices.SelectAndConfirmPrice();
                Quantity.AddQuantity();
            }
            else
            {
                SelectEditCatalogueSolution.EditSolution(newSolutionName, newAdditionalServiceName);

                SelectCatalogueSolutionServiceRecipients.AddCatalogueSolutionServiceRecipient();
                SelectAndConfirmPrices.SelectAndConfirmPrice();
                Quantity.AddQuantity();

                if (HasAdditionalService(newSolutionName) && !string.IsNullOrWhiteSpace(newAdditionalServiceName))
                {
                    SelectAdditionalServiceRecipients.AddServiceRecipients();
                    SelectAndConfirmAdditionalServicePrice.SelectAndConfirmPrice();
                    Quantity.AddQuantity();
                }

                if (HasAssociatedServices(newSolutionName))
                {
                    if (!string.IsNullOrWhiteSpace(newAssociatedService))
                    {
                        SelectEditAssociatedService.AddAssociatedService("Yes", newAssociatedService);
                        SelectAssociatedServiceRecipents.AddServiceRecipient();
                        SelectAndConfirmAssociatedServicePrices.SelectAndConfirmPrice();
                        Quantity.AddQuantity();
                    }
                    else
                    {
                        SelectEditAssociatedService.AddAssociatedService();
                    }
                }
            }

            SolutionAndServicesReview.ReviewSolutionAndServices();

            TaskList.SelectFundingSourcesTask();
            SelectFundingSources.AddFundingSources(newSolutionName, newAdditionalServiceName, newAssociatedService, isAssociatedServiceOnlyOrder);
        }

        public void EditAdditionalService(string solutionName, string newAdditionalService, string newAssociatedService = "", string oldAdditionalService = "")
        {
            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnly();

            TaskList.EditSolutionsAndServicesTask(isAssociatedServiceOnlyOrder);

            SelectEditCatalogueSolution.EditAdditionalService(solutionName, oldAdditionalService, newAdditionalService, HasTheOriginalOrderAdditionalService());

            if (!HasAdditionalService(solutionName))
            {
                return;
            }

            if (HasAdditionalService(solutionName) && !string.IsNullOrWhiteSpace(newAdditionalService))
            {
                SelectAdditionalServiceRecipients.AddServiceRecipients();
                SelectAndConfirmAdditionalServicePrice.SelectAndConfirmPrice();
                Quantity.AddQuantity();
            }

            if (HasAssociatedServices(solutionName))
            {
                if (!string.IsNullOrWhiteSpace(newAssociatedService))
                {
                    SelectEditAssociatedService.AddAssociatedService("Yes", newAssociatedService);
                    SelectAssociatedServiceRecipents.AddServiceRecipient();
                    SelectAndConfirmAssociatedServicePrices.SelectAndConfirmPrice();
                    Quantity.AddQuantity();
                }
                else
                {
                    if (!HasTheOriginalOrderAssociatedService())
                    {
                        SelectEditAssociatedService.AddAssociatedService();
                    }
                }

                SolutionAndServicesReview.ReviewSolutionAndServices();

                TaskList.SelectFundingSourcesTask();
                SelectFundingSources.AddFundingSources(solutionName, newAdditionalService, newAssociatedService, isAssociatedServiceOnlyOrder);
            }
        }

        public void EditAssociatedService(string solutionName, string newAssociatedServiceName, string additionalServiceName = "", string oldAssociatedServiceName = "")
        {
            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnly();

            TaskList.EditSolutionsAndServicesTask(isAssociatedServiceOnlyOrder);

            SelectEditAssociatedService.EditAssociatedService(solutionName, newAssociatedServiceName, HasTheOriginalOrderAssociatedService(), oldAssociatedServiceName);

            if (HasAssociatedServices(solutionName) && !string.IsNullOrWhiteSpace(newAssociatedServiceName))
            {
                SelectAssociatedServiceRecipents.AddServiceRecipient();
                SelectAndConfirmAssociatedServicePrices.SelectAndConfirmPrice();
                Quantity.AddQuantity();
            }

            SolutionAndServicesReview.ReviewSolutionAndServices();

            TaskList.SelectFundingSourcesTask();
            SelectFundingSources.AddFundingSources(solutionName, additionalServiceName, newAssociatedServiceName, isAssociatedServiceOnlyOrder);
        }

        private bool IsAssociatedServiceOnly()
        {
            using var dbContext = Factory.DbContext;
            var orderID = Driver.Url.Split('/').Last().Split('-')[0].Replace("C0", string.Empty);

            return dbContext.Orders.Any(o => string.Equals(o.Id.ToString(), orderID) && o.AssociatedServicesOnly);
        }

        private bool HasAdditionalService(string solutionName)
        {
            using var dbContext = Factory.DbContext;

            return dbContext.AdditionalServices.Any(a => a.Solution.CatalogueItem.Name == solutionName);
        }

        private bool HasAssociatedServices(string solutionName)
        {
            using var dbContext = Factory.DbContext;

            return dbContext.SupplierServiceAssociations.Any(ssa => ssa.CatalogueItem.Name == solutionName);
        }

        private bool HasTheOriginalOrderAdditionalService()
        {
            using var dbContext = Factory.DbContext;

            var index = Driver.Url.Split('/').Count() - 2;

            var orderID = Driver.Url.Split('/').ElementAt(index).Split('-')[0].Replace("C0", string.Empty);

            var result = dbContext.Orders.Any(o => string.Equals(o.Id.ToString(), orderID) && o.OrderItems.Any(i => i.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService));

            return result;
        }

        private bool HasTheOriginalOrderAssociatedService()
        {
            using var dbContext = Factory.DbContext;

            var index = Driver.Url.Split('/').Count() - 2;

            var orderID = Driver.Url.Split('/').ElementAt(index).Split('-')[0].Replace("C0", string.Empty);

            var result = dbContext.Orders.Any(o => string.Equals(o.Id.ToString(), orderID) && o.OrderItems.Any(i => i.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService));

            return result;
        }
    }
}
