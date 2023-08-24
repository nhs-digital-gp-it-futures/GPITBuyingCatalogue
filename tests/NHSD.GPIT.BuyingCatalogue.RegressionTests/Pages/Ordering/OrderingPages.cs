using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Dashboard;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.Step_Five;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepOne;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepThree;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.AssociatedServiceOnly;
using NHSD.GPIT.BuyingCatalogue.RegressionTests.Pages.Ordering.StepTwo.DeliveryDates;
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
            PlannedDeliveryDates = new PlannedDeliveryDates(driver, commonActions);
            SelectFundingSources = new SelectFundingSources(driver, commonActions);
            SelectSupplier = new SelectSupplier(driver, commonActions);
            SupplierContacts = new SupplierContacts(driver, commonActions);
            SelectEditCatalogueSolution = new SelectEditCatalogueSolution(driver, commonActions, factory);
            SelectEditCatalogueSolutionServiceRecipients = new SelectEditCatalogueSolutionServiceRecipients(driver, commonActions, factory);
            SelectEditAndConfirmPrices = new SelectEditAndConfirmPrices(driver, commonActions, factory);
            Quantity = new Quantity(driver, commonActions, factory);
            SolutionAndServicesReview = new SolutionAndServicesReview(driver, commonActions);
            SelectEditAssociatedService = new SelectEditAssociatedService(driver, commonActions, factory);
            SelectEditAssociatedServiceRecipents = new SelectEditAssociatedServiceRecipents(driver, commonActions, factory);
            SelectEditAndConfirmAssociatedServicePrices = new SelectEditAndConfirmAssociatedServicePrices(driver, commonActions, factory);
            SelectEditAdditionalServiceRecipients = new SelectEditAdditionalServiceRecipients(driver, commonActions, factory);
            SelectEditAndConfirmAdditionalServicePrice = new SelectEditAndConfirmAdditionalServicePrice(driver, commonActions, factory);
            SelectEditAssociatedServiceOnly = new SelectEditAssociatedServiceOnly(driver, commonActions);
            SelectEditAssociatedServiceRecipientOnly = new SelectEditAssociatedServiceRecipientOnly(driver, commonActions, factory);
            SelectEditAndConfirmAssociatedServiceOnlyPrices = new SelectEditAndConfirmAssociatedServiceOnlyPrices(driver, commonActions, factory);
            SelectSolutionAndServices = new SelectSolutionAndServices(driver, commonActions);
            OrderingStepThree = new OrderingStepThree(driver, commonActions);
            OrderingStepFour = new OrderingStepFour(driver, commonActions);
            ImportServiceReceipients = new ImportServiceReceipients(driver, commonActions);
            ConfirmServieReceipients = new ConfirmServieReceipients(driver, commonActions);
            AmendOrder = new AmendOrder(driver, commonActions, factory);
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

        internal PlannedDeliveryDates PlannedDeliveryDates { get; }

        internal SelectFundingSources SelectFundingSources { get; }

        internal SelectSupplier SelectSupplier { get; }

        internal SupplierContacts SupplierContacts { get; }

        internal SelectEditCatalogueSolution SelectEditCatalogueSolution { get; }

        internal SelectEditCatalogueSolutionServiceRecipients SelectEditCatalogueSolutionServiceRecipients { get; }

        internal SelectEditAndConfirmPrices SelectEditAndConfirmPrices { get; }

        internal Quantity Quantity { get; }

        internal SolutionAndServicesReview SolutionAndServicesReview { get; }

        internal SelectEditAssociatedService SelectEditAssociatedService { get; }

        internal SelectEditAssociatedServiceRecipents SelectEditAssociatedServiceRecipents { get; }

        internal SelectEditAndConfirmAssociatedServicePrices SelectEditAndConfirmAssociatedServicePrices { get; }

        internal SelectEditAdditionalServiceRecipients SelectEditAdditionalServiceRecipients { get; }

        internal SelectEditAndConfirmAdditionalServicePrice SelectEditAndConfirmAdditionalServicePrice { get; }

        internal SelectEditAssociatedServiceOnly SelectEditAssociatedServiceOnly { get; }

        internal SelectSolutionAndServices SelectSolutionAndServices { get; }

        internal SelectEditAssociatedServiceRecipientOnly SelectEditAssociatedServiceRecipientOnly { get; }

        internal SelectEditAndConfirmAssociatedServiceOnlyPrices SelectEditAndConfirmAssociatedServiceOnlyPrices { get; }

        internal OrderingStepThree OrderingStepThree { get; }

        internal OrderingStepFour OrderingStepFour { get; }

        internal ImportServiceReceipients ImportServiceReceipients { get; }

        internal IWebDriver Driver { get; }

        internal ConfirmServieReceipients ConfirmServieReceipients { get; }

        internal AmendOrder AmendOrder { get; }

        public void StepOnePrepareOrder(
            string supplierName,
            string orderDescription,
            bool addNewSupplierContact = false,
            EntityFramework.Ordering.Models.OrderTriageValue orderTriage = EntityFramework.Ordering.Models.OrderTriageValue.Under40K,
            EntityFramework.Catalogue.Models.CatalogueItemType itemType = EntityFramework.Catalogue.Models.CatalogueItemType.Solution)
        {
            TaskList.OrderDescriptionTask();
            OrderingStepOne.AddOrderDescription(orderDescription);

            TaskList.CallOffOrderingPartyContactDetailsTask();
            OrderingStepOne.AddCallOffOrderingPartyContactDetails();

            TaskList.SupplierInformationAndContactDetailsTask();
            SelectSupplier.SelectAndConfirmSupplier(supplierName);
            SupplierContacts.ConfirmContact(addNewSupplierContact);

            TaskList.TimescalesForCallOffAgreementTask();
            OrderingStepOne.AddTimescaleForCallOffAgreement(orderTriage, itemType);
        }

        public void StepTwoAddSolutionsAndServices(string solutionName, string additionalService = "", string associatedService = "", int multipleServiceRecipients = 0 , bool importServiceRecipients = false, string fileName = "", bool allServiceRecipients = false)
        {
            StepTwoAddSolutionsAndServices(solutionName, new List<string> { additionalService }, new List<string> { associatedService }, multipleServiceRecipients, importServiceRecipients, fileName, allServiceRecipients);
        }

        public void StepTwoAddSolutionsAndServices(string solutionName, IEnumerable<string>? additionalServices, string associatedService = "", int multipleServiceRecipients = 0, bool importServiceRecipients = false, string fileName = "")
        {
            StepTwoAddSolutionsAndServices(solutionName, additionalServices, new List<string> { associatedService }, multipleServiceRecipients, importServiceRecipients, fileName);
        }

        public void StepTwoAddSolutionsAndServices(string solutionName, string additionalService, IEnumerable<string>? associatedServices, int multipleServiceRecipients = 0, bool importServiceRecipients = false, string fileName = "")
        {
            StepTwoAddSolutionsAndServices(solutionName, new List<string> { additionalService }, associatedServices, multipleServiceRecipients, importServiceRecipients, fileName);
        }

        public void StepTwoAddSolutionsAndServices(string solutionName, IEnumerable<string>? additionalServices, IEnumerable<string>? associatedServices, int multipleServiceRecipients = 0, bool importServiceRecipients = false, string fileName = "", bool allServiceRecipients = false)
        {
            var orderId = OrderID();
            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnlyOrder(orderId);

            TaskList.SelectSolutionsAndServicesTask(isAssociatedServiceOnlyOrder);

            if (!isAssociatedServiceOnlyOrder)
            {
                SelectEditCatalogueSolution.SelectSolution(solutionName, additionalServices);

                if (!importServiceRecipients)
                {
                    SelectEditCatalogueSolutionServiceRecipients.AddCatalogueSolutionServiceRecipient(multipleServiceRecipients, allServiceRecipients);
                }
                else
                {
                    ImportServiceReceipients.ImportServiceRecipients(fileName);
                }

                ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
                SelectEditAndConfirmPrices.SelectAndConfirmPrice();
                Quantity.AddQuantity();

                if (HasAdditionalService(solutionName) && additionalServices != default && additionalServices.All(a => !string.IsNullOrWhiteSpace(a)))
                {
                    foreach (var additionalService in additionalServices)
                    {
                        if (!importServiceRecipients)
                        {
                            SelectEditAdditionalServiceRecipients.AddServiceRecipients();
                        }
                        else
                        {
                            ImportServiceReceipients.ImportServiceRecipients(fileName);
                        }

                        ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
                        SelectEditAndConfirmAdditionalServicePrice.SelectAndConfirmPrice();
                        Quantity.AddQuantity();
                    }
                }

                if (HasAssociatedServices(solutionName))
                {
                    if ((associatedServices != default) && associatedServices.All(a => !string.IsNullOrWhiteSpace(a)))
                    {
                        SelectEditAssociatedService.AddAssociatedService(associatedServices, "Yes");

                        foreach (var associatedService in associatedServices)
                        {
                            if (!importServiceRecipients)
                            {
                                SelectEditAssociatedServiceRecipents.AddServiceRecipient();
                            }
                            else
                            {
                                ImportServiceReceipients.ImportServiceRecipients(fileName);
                            }

                            ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
                            SelectEditAndConfirmAssociatedServicePrices.SelectAndConfirmPrice();
                            Quantity.AddQuantity();
                        }
                    }
                    else
                    {
                        SelectEditAssociatedService.AddAssociatedService();
                    }
                }
            }
            else
            {
                SelectEditAssociatedServiceOnly.SelectAssociatedServices(solutionName, associatedServices);

                if ((associatedServices != default) && associatedServices.All(a => !string.IsNullOrWhiteSpace(a)))
                {
                    foreach (var associatedService in associatedServices)
                    {
                        if (!importServiceRecipients)
                        {
                            SelectEditAssociatedServiceRecipientOnly.AddServiceRecipient(multipleServiceRecipients);
                        }
                        else
                        {
                            ImportServiceReceipients.ImportServiceRecipients(fileName);
                        }

                        ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
                        SelectEditAndConfirmAssociatedServiceOnlyPrices.SelectAndConfirmPrice();
                        Quantity.AddQuantity();
                    }
                }
            }

            SolutionAndServicesReview.ReviewSolutionAndServices();

            TaskList.SelectPlannedDeliveryDatesTask();
            PlannedDeliveryDates.PlannedDeliveryDate(solutionName, isAssociatedServiceOnlyOrder, associatedServices, additionalServices);

            var isMultiFramework = IsMultiFramework(orderId);

            if (isMultiFramework)
            {
                TaskList.SelectFrameWork();
                SelectFundingSources.AddFundingSources(solutionName, isAssociatedServiceOnlyOrder, associatedServices, additionalServices);
            }
            else
            {
                var isLocalFundingOnly = IsLocalFundingOnly(orderId);
                if (isLocalFundingOnly)
                {
                    TaskList.SelectLocalFundingSourcesTask();
                }
                else
                {
                    TaskList.SelectFundingSourcesTask();
                    SelectFundingSources.AddFundingSources(solutionName, isAssociatedServiceOnlyOrder, associatedServices, additionalServices);
                }
            }
        }

        public void EditPlannedDeliveryDate(string solutionName, string additionalService, string associatedService, bool editplanneddeliverydate = true)
        {
            var orderId = OrderID();
            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnlyOrder(orderId);

            TaskList.EditPlannedDeliveryDateTask();
            PlannedDeliveryDates.EditPlannedDeliveryDate(solutionName, isAssociatedServiceOnlyOrder, additionalService, associatedService, editplanneddeliverydate);
        }

        public void StepThreeCompleteContract(bool isDefault = true)
        {
            TaskList.ImplementationPlanMilestonesTask();
            OrderingStepThree.SelectImplementationPlan(isDefault);

            using var dbContext = Factory.DbContext;
            var orderNumber = Driver.Url.Split('/').Last().Split('-')[0].Replace("C0", string.Empty);

            var isOrderWithAssociatedService = dbContext.Orders
                .Any(o => string.Equals(o.OrderNumber.ToString(), orderNumber) &&
                o.OrderItems.Any(i => i.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService));

            if (isOrderWithAssociatedService)
            {
                TaskList.AssociatedServiceBillingAndRequirementsTask();
                OrderingStepThree.SelectAssociatedServicesBilling();
                //StepThreeContractAssociatedServices();
            }

            TaskList.DataProcessingInformationTask();
        }

        public void StepThreeContractAssociatedServices(bool isDefault = true)
        {
            TaskList.AssociatedServiceBillingAndRequirementsTask();
            OrderingStepThree.SelectAssociatedServicesBilling(isDefault);
            TaskList.DataProcessingInformationTask();
        }

        public void StepFourReviewAndCompleteOrder()
        {
            TaskList.ReviewAndCompleteOrderTask();
            OrderingStepFour.ReviewAndCompleteOrder();
        }

        public void StepFiveAmendOrder()
        {
            AmendOrder.AmendOrderClickAmend();
        }

        public void EditCatalogueSolution(string newSolutionName, string newAdditionalServiceName = "", string newAssociatedService = "", int multipleServiceRecipients = 0, bool allServiceRecipients = false)
        {
           EditCatalogueSolution(newSolutionName, new List<string> { newAdditionalServiceName }, new List<string> { newAssociatedService }, multipleServiceRecipients);
        }

        public void EditCatalogueSolution(string newSolutionName, IEnumerable<string>? newAdditionalServiceNames, string newAssociatedService = "", int multipleServiceRecipients = 0)
        {
            EditCatalogueSolution(newSolutionName, newAdditionalServiceNames, new List<string> { newAssociatedService }, multipleServiceRecipients);
        }

        public void EditCatalogueSolution(string newSolutionName, string newAdditionalServiceName, IEnumerable<string>? newAssociatedServices, int multipleServiceRecipients = 0)
        {
            EditCatalogueSolution(newSolutionName, new List<string> { newAdditionalServiceName }, newAssociatedServices, multipleServiceRecipients);
        }

        public void EditCatalogueSolution(string newSolutionName, IEnumerable<string>? newAdditionalServiceNames, IEnumerable<string>? newAssociatedServices, int multipleServiceRecipients = 0, bool allServiceRecipients = false)
        {
            var orderId = OrderID();
            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnlyOrder(orderId);

            TaskList.EditSolutionsAndServicesTask(isAssociatedServiceOnlyOrder);

            if (isAssociatedServiceOnlyOrder)
            {
                SelectEditAssociatedServiceOnly.EditSolutionForAssociatedService(newSolutionName, newAssociatedServices);

                if (newAssociatedServices != default && newAssociatedServices.All(a => !string.IsNullOrWhiteSpace(a)))
                {
                    foreach (var associatedService in newAssociatedServices)
                    {
                        SelectEditAssociatedServiceRecipientOnly.AddServiceRecipient(multipleServiceRecipients);
                        ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
                        SelectEditAndConfirmAssociatedServiceOnlyPrices.SelectAndConfirmPrice();
                        Quantity.AddQuantity();
                    }
                }
            }
            else
            {
                SelectEditCatalogueSolution.EditSolution(newSolutionName, newAdditionalServiceNames);

                SelectEditCatalogueSolutionServiceRecipients.AddCatalogueSolutionServiceRecipient(multipleServiceRecipients,allServiceRecipients);
                ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
                SelectEditAndConfirmPrices.SelectAndConfirmPrice();
                Quantity.AddQuantity();

                if (HasAdditionalService(newSolutionName) && newAdditionalServiceNames != default && newAdditionalServiceNames.All(a => !string.IsNullOrWhiteSpace(a)))
                {
                    foreach (var additionalService in newAdditionalServiceNames)
                    {
                        SelectEditAdditionalServiceRecipients.AddServiceRecipients();
                        ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
                        SelectEditAndConfirmAdditionalServicePrice.SelectAndConfirmPrice();
                        Quantity.AddQuantity();
                    }
                }

                if (HasAssociatedServices(newSolutionName))
                {
                    if (newAssociatedServices != default && newAssociatedServices.All(a => !string.IsNullOrWhiteSpace(a)))
                    {
                        SelectEditAssociatedService.AddAssociatedService(newAssociatedServices, "Yes");
                        foreach (var associatedService in newAssociatedServices)
                        {
                            SelectEditAssociatedServiceRecipents.AddServiceRecipient();
                            ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
                            SelectEditAndConfirmAssociatedServicePrices.SelectAndConfirmPrice();
                            Quantity.AddQuantity();
                        }
                    }
                    else
                    {
                        SelectEditAssociatedService.AddAssociatedService();
                    }
                }
            }

            SolutionAndServicesReview.ReviewSolutionAndServices();

            TaskList.SelectFundingSourcesTask();
            SelectFundingSources.AddFundingSources(newSolutionName, isAssociatedServiceOnlyOrder, newAssociatedServices, newAdditionalServiceNames);
        }

        public void EditAdditionalService(string solutionName, string newAdditionalService, string newAssociatedService = "", string oldAdditionalService = "")
        {
            EditAdditionalService(solutionName, new List<string> { newAdditionalService }, new List<string> { newAssociatedService }, new List<string> { oldAdditionalService });
        }

        public void EditAdditionalService(string solutionName, IEnumerable<string> newAdditionalServices, string newAssociatedService = "", string oldAdditionalService = "")
        {
            EditAdditionalService(solutionName, newAdditionalServices, new List<string> { newAssociatedService }, new List<string> { oldAdditionalService });
        }

        public void EditAdditionalService(string solutionName, IEnumerable<string> newAdditionalServices, IEnumerable<string>? newAssociatedServices, IEnumerable<string>? oldAdditionalServices)
        {
            var orderId = OrderID();
            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnlyOrder(orderId);

            TaskList.EditSolutionsAndServicesTask(isAssociatedServiceOnlyOrder);

            SelectEditCatalogueSolution.EditAdditionalService(solutionName, oldAdditionalServices, newAdditionalServices, HasTheOriginalOrderAdditionalService(orderId));

            if (!HasAdditionalService(solutionName))
            {
                return;
            }

            if (HasAdditionalService(solutionName) && newAdditionalServices.All(a => !string.IsNullOrWhiteSpace(a)))
            {
                foreach (var additionalService in newAdditionalServices)
                {
                    SelectEditAdditionalServiceRecipients.AddServiceRecipients();
                    ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
                    SelectEditAndConfirmAdditionalServicePrice.SelectAndConfirmPrice();
                    Quantity.AddQuantity();
                }
            }

            if (HasAssociatedServices(solutionName))
            {
                if (newAssociatedServices != default && newAssociatedServices.All(a => !string.IsNullOrWhiteSpace(a)))
                {
                    SelectEditAssociatedService.AddAssociatedService(newAssociatedServices, "Yes");
                    SelectEditAssociatedServiceRecipents.AddServiceRecipient();
                    ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
                    SelectEditAndConfirmAssociatedServicePrices.SelectAndConfirmPrice();
                    Quantity.AddQuantity();
                }
                else
                {
                    if (!HasTheOriginalOrderAssociatedService(orderId))
                    {
                        SelectEditAssociatedService.AddAssociatedService();
                    }
                }

                SolutionAndServicesReview.ReviewSolutionAndServices();

                TaskList.SelectFundingSourcesTask();
                SelectFundingSources.AddFundingSources(solutionName, isAssociatedServiceOnlyOrder, newAssociatedServices, newAdditionalServices);
            }
        }

        public void EditAssociatedService(string solutionName, string newAssociatedServiceName, string additionalServiceName = "", string oldAssociatedServiceName = "")
        {
            EditAssociatedService(solutionName, new List<string> { newAssociatedServiceName }, new List<string> { additionalServiceName }, new List<string> { oldAssociatedServiceName });
        }

        public void EditAssociatedService(string solutionName, IEnumerable<string> newAssociatedServices, string additionalServiceName = "", string oldAssociatedServiceName = "")
        {
            EditAssociatedService(solutionName, newAssociatedServices, new List<string> { additionalServiceName }, new List<string> { oldAssociatedServiceName });
        }

        public void EditAssociatedService(string solutionName, IEnumerable<string> newAssociatedServices, IEnumerable<string>? additionalServices, IEnumerable<string>? oldAssociatedServices)
        {
            var orderId = OrderID();
            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnlyOrder(orderId);

            TaskList.EditSolutionsAndServicesTask(isAssociatedServiceOnlyOrder);

            SelectEditAssociatedService.EditAssociatedService(solutionName, newAssociatedServices, HasTheOriginalOrderAssociatedService(orderId), oldAssociatedServices);

            if (HasAssociatedServices(solutionName) && newAssociatedServices.All(a => !string.IsNullOrWhiteSpace(a)))
            {
                foreach (var associatedService in newAssociatedServices)
                {
                    SelectEditAssociatedServiceRecipents.AddServiceRecipient();
                    ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
                    SelectEditAndConfirmAssociatedServicePrices.SelectAndConfirmPrice();
                    Quantity.AddQuantity();
                }
            }

            SolutionAndServicesReview.ReviewSolutionAndServices();

            TaskList.SelectFundingSourcesTask();
            SelectFundingSources.AddFundingSources(solutionName, isAssociatedServiceOnlyOrder, newAssociatedServices, additionalServices);
        }

        public void EditAssociatedServiceOnly(string solutionName, string newAssociatedServiceName, string oldAssociatedServiceName, int multipleServiceRecipients = 0)
        {
            EditAssociatedServiceOnly(solutionName, new List<string> { newAssociatedServiceName }, new List<string> { oldAssociatedServiceName }, multipleServiceRecipients);
        }

        public void EditAssociatedServiceOnly(string solutionName, IEnumerable<string> newAssociatedServices, string oldAssociatedServiceName, int multipleServiceRecipients = 0)
        {
            EditAssociatedServiceOnly(solutionName, newAssociatedServices, new List<string> { oldAssociatedServiceName }, multipleServiceRecipients);
        }

        public void EditAssociatedServiceOnly(string solutionName, IEnumerable<string> newAssociatedServices, IEnumerable<string> oldAssociatedServices, int multipleServiceRecipients = 0)
        {
            var orderId = OrderID();
            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnlyOrder(orderId);

            TaskList.EditSolutionsAndServicesTask(isAssociatedServiceOnlyOrder);

            SelectEditAssociatedServiceOnly.EditAssociatedServiceOnly(newAssociatedServices, oldAssociatedServices);

            foreach (var associatedService in newAssociatedServices)
            {
                SelectEditAssociatedServiceRecipientOnly.AddServiceRecipient(multipleServiceRecipients);
                ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
                SelectEditAndConfirmAssociatedServiceOnlyPrices.SelectAndConfirmPrice();
                Quantity.AddQuantity();
            }

            SolutionAndServicesReview.ReviewSolutionAndServices();

            TaskList.SelectFundingSourcesTask();
            SelectFundingSources.AddFundingSources(solutionName, isAssociatedServiceOnlyOrder, newAssociatedServices, null);
        }

        public void EditCatalogueSolutionServiceRecipient(string solutionName)
        {
            var orderId = OrderID();
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder(orderId));

            SelectEditCatalogueSolutionServiceRecipients.EditCatalogueSolutionServiceRecipient(solutionName);
            ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
            SelectEditAndConfirmAdditionalServicePrice.EditConfirmPrice();

            Quantity.AddQuantity();
            TaskList.EditCatalogueAdditionalAndAssociatedServiceTask();
        }

        public void AmendOrderDescription(string amendOrderDescription)
        {
            TaskList.AmendOrderDescriptionTask(amendOrderDescription);
            OrderingStepOne.AddOrderDescription(amendOrderDescription);
        }

        public void EditCatalogueSolutionPrice(string solutionName)
        {
            var orderId = OrderID();
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder(orderId));

            SelectEditAndConfirmPrices.EditCatalogueSolutionPrice(solutionName);
        }

        public void EditAdditionalServiceRecipient(string additionalServiceName)
        {
            var orderId = OrderID();
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder(orderId));

            SelectEditAdditionalServiceRecipients.EditAdditionalServiceRecipient(additionalServiceName);
            ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
            SelectEditAndConfirmAdditionalServicePrice.EditConfirmPrice();

            Quantity.AddQuantity();
            TaskList.EditCatalogueAdditionalAndAssociatedServiceTask();
        }

        public void EditAdditionalServicePrice(string additionalServiceName)
        {
            var orderId = OrderID();
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder(orderId));

            SelectEditAndConfirmAdditionalServicePrice.EditAdditionalServicePrice(additionalServiceName);
        }

        public void EditCatalogueItemQuantity(string catalogueItemName)
        {
            var orderId = OrderID();
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder(orderId));

            Quantity.EditQuantity(catalogueItemName);
        }

        public void EditAssociatedServiceRecipient(string associatedServiceName)
        {
            var orderId = OrderID();
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder(orderId));

            SelectEditAssociatedServiceRecipents.EditServiceRecipient(associatedServiceName);
            ConfirmServieReceipients.ConfirmServiceReceipientsChanges();

            TaskList.EditCatalogueAdditionalAndAssociatedServiceTask();
        }

        public void EditAssociatedServicePrice(string associatedServiceName)
        {
            var orderId = OrderID();
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder(orderId));

            SelectEditAndConfirmAssociatedServicePrices.EditAssociatedServicePrice(associatedServiceName);
        }

        public void EditAssociatedServiceOnlyServiceRecipients(string associatedServiceName)
        {
            var orderId = OrderID();
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder(orderId));

            SelectEditAssociatedServiceRecipientOnly.EditServiceRecipient(associatedServiceName);
            ConfirmServieReceipients.ConfirmServiceReceipientsChanges();

            TaskList.EditCatalogueAdditionalAndAssociatedServiceTask();
        }

        public void EditAssociatedServiceOnlyPrice(string associatedServiceName)
        {
            var orderId = OrderID();
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder(orderId));

            SelectEditAndConfirmAssociatedServiceOnlyPrices.EditPrice(associatedServiceName);
        }

        public void AmendSolutionsAndServices(string solutionName, string additionalService = "", string associatedService = "", int multipleServiceRecipients = 0, bool importServiceRecipients = false, string fileName = "")
        {
            AmendSolutionsAndServices(solutionName, new List<string> { additionalService }, new List<string> { associatedService }, multipleServiceRecipients, importServiceRecipients, fileName);
        }

        public void AmendSolutionsAndServices(string solutionName, IEnumerable<string>? additionalServices, IEnumerable<string>? associatedServices, int multipleServiceRecipients = 0, bool importServiceRecipients = false, string fileName = "")
        {
            var orderId = OrderID();

            TaskList.AmendSolutionAndServicesTask();

            if (!importServiceRecipients)
            {
                SelectEditCatalogueSolutionServiceRecipients.AmendEditCatalogueSolutionServiceRecipient(solutionName, multipleServiceRecipients);
            }
            else
            {
                SelectEditCatalogueSolutionServiceRecipients.AmendImportServiceRecipients(solutionName, fileName);
            }

            ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
            SelectEditAndConfirmPrices.AmendViewAndConfirmPrice();
            Quantity.AddQuantity();
            PlannedDeliveryDates.AmendPlannedDeliveryDate(solutionName);

            if (HasAdditionalService(solutionName) && additionalServices != default && additionalServices.All(a => !string.IsNullOrWhiteSpace(a)))
            {
                foreach (var additionalService in additionalServices)
                {
                    SelectEditAdditionalServiceRecipients.AmendEditAdditionalServiceRecipient(additionalService);

                    ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
                    SelectEditAndConfirmPrices.AmendViewAndConfirmPrice();
                    Quantity.AddQuantity();
                    PlannedDeliveryDates.AmendPlannedDeliveryDate(additionalService);
                }
            }

            SolutionAndServicesReview.AmendReviewSolutionAndServices();
            TaskList.SelectFundingSourcesTask();
            SelectFundingSources.AmendAddFundingSources(solutionName, additionalServices);

            StepThreeCompleteContract();
            StepFourReviewAndCompleteOrder();
        }

        public void AmendAddSolutionsAndServices(string solutionName, string additionalService = "", int multipleServiceRecipients = 0, bool importServiceRecipients = false, string fileName = "")
        {
            AmendAddSolutionsAndServices(solutionName, new List<string> { additionalService }, multipleServiceRecipients, importServiceRecipients, fileName);
        }

        public void AmendAddSolutionsAndServices(string solutionName, IEnumerable<string>? additionalServices, int multipleServiceRecipients = 0, bool importServiceRecipients = false, string fileName = "")
        {
            var orderId = OrderID();

            TaskList.AmendSolutionAndServicesTask();

            SelectEditCatalogueSolutionServiceRecipients.AmendEditCatalogueSolutionServiceRecipient(solutionName, multipleServiceRecipients);
            ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
            SelectEditAndConfirmPrices.AmendViewAndConfirmPrice();
            Quantity.AddQuantity();
            PlannedDeliveryDates.AmendPlannedDeliveryDate(solutionName);

            if (HasAdditionalService(solutionName) && additionalServices != default && additionalServices.All(a => !string.IsNullOrWhiteSpace(a)))
            {
                foreach (var additionalService in additionalServices)
                {
                    SelectEditCatalogueSolution.AddAdditionalServices(additionalService);
                    SelectEditAdditionalServiceRecipients.AddServiceRecipients();

                    ConfirmServieReceipients.ConfirmServiceReceipientsChanges();
                    SelectEditAndConfirmAdditionalServicePrice.SelectAndConfirmPrice();
                    Quantity.AddQuantity();
                    PlannedDeliveryDates.AmendPlannedDeliveryDate(additionalService);
                }
            }

            SolutionAndServicesReview.AmendReviewSolutionAndServices();
            TaskList.SelectFundingSourcesTask();
            SelectFundingSources.AmendAddFundingSources(solutionName, additionalServices);

            StepThreeCompleteContract();
            StepFourReviewAndCompleteOrder();
        }

        private bool IsAssociatedServiceOnlyOrder(int orderId)
        {
            using var dbContext = Factory.DbContext;

            var result = dbContext.Orders
               .Any(o => o.Id == orderId && o.AssociatedServicesOnly == true);

            return result;
        }

        private int OrderID()
        {
            using var dbContext = Factory.DbContext;

            var callOffId = CallOffId.Parse(Driver.Url.Split('/').Last()).Id;
            var orderId = dbContext.OrderId(callOffId).Result;

            return orderId;
        }

        private bool IsMultiFramework(int orderId)
        {
            using var dbContext = Factory.DbContext;

            var frameworks = dbContext.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .SelectMany(oi => oi.CatalogueItem.Solution.FrameworkSolutions.Select(fs => fs.Framework))
                .ToList();

            return frameworks.Count > 1;
        }

        private bool IsLocalFundingOnly(int orderId)
        {
            using var dbContext = Factory.DbContext;
            var orderID = Driver.Url.Split('/').Last().Split('-')[0].Replace("C0", string.Empty);

            var frameworks = dbContext.OrderItems
             .Where(oi => oi.OrderId == orderId)
                .SelectMany(oi => oi.CatalogueItem.Solution.FrameworkSolutions.Select(fs => fs.Framework))
                .ToList();

            if (frameworks.Count == 0 || frameworks.Count > 1)
            {
                return false;
            }

            return frameworks.First().LocalFundingOnly;
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

        private bool HasTheOriginalOrderAdditionalService(int orderId)
        {
            using var dbContext = Factory.DbContext;

            var result = dbContext.Orders
                .Any(o => o.Id == orderId
                    && o.OrderItems.Any(i => i.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService));

            return result;
        }

        private bool HasTheOriginalOrderAssociatedService(int orderId)
        {
            using var dbContext = Factory.DbContext;

            var result = dbContext.Orders
                .Any(o => o.Id == orderId
                    && o.OrderItems.Any(i => i.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService));

            return result;
        }
    }
}
