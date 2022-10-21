﻿using NHSD.GPIT.BuyingCatalogue.E2ETests.Framework.Actions.Common;
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

        internal IWebDriver Driver { get; }

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

        public void StepTwoAddSolutionsAndServices(string solutionName, string additionalService = "", string associatedService = "", bool multipleServiceRecipients = false)
        {
            StepTwoAddSolutionsAndServices(solutionName, new List<string> { additionalService }, new List<string> { associatedService }, multipleServiceRecipients);
        }

        public void StepTwoAddSolutionsAndServices(string solutionName, IEnumerable<string>? additionalServices, string associatedService = "", bool multipleServiceRecipients = false)
        {
            StepTwoAddSolutionsAndServices(solutionName, additionalServices, new List<string> { associatedService }, multipleServiceRecipients);
        }

        public void StepTwoAddSolutionsAndServices(string solutionName, string additionalService, IEnumerable<string>? associatedServices, bool multipleServiceRecipients = false)
        {
            StepTwoAddSolutionsAndServices(solutionName, new List<string> { additionalService }, associatedServices, multipleServiceRecipients);
        }

        public void StepTwoAddSolutionsAndServices(string solutionName, IEnumerable<string>? additionalServices, IEnumerable<string>? associatedServices, bool multipleServiceRecipients = false)
        {
            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnlyOrder();

            TaskList.SelectSolutionsAndServicesTask(isAssociatedServiceOnlyOrder);

            if (!isAssociatedServiceOnlyOrder)
            {
                SelectEditCatalogueSolution.SelectSolution(solutionName, additionalServices);

                SelectEditCatalogueSolutionServiceRecipients.AddCatalogueSolutionServiceRecipient(multipleServiceRecipients);
                SelectEditAndConfirmPrices.SelectAndConfirmPrice();
                Quantity.AddQuantity();

                if (HasAdditionalService(solutionName) && additionalServices != default && additionalServices.All(a => !string.IsNullOrWhiteSpace(a)))
                {
                        foreach (var additionalService in additionalServices)
                        {
                                SelectEditAdditionalServiceRecipients.AddServiceRecipients();
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
                            SelectEditAssociatedServiceRecipents.AddServiceRecipient();
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
                        SelectEditAssociatedServiceRecipientOnly.AddServiceRecipient(multipleServiceRecipients);
                        SelectEditAndConfirmAssociatedServiceOnlyPrices.SelectAndConfirmPrice();
                        Quantity.AddQuantity();
                    }
                }
            }

            SolutionAndServicesReview.ReviewSolutionAndServices();

            TaskList.SelectPlannedDeliveryDatesTask();
            PlannedDeliveryDates.PlannedDeliveryDate(solutionName, isAssociatedServiceOnlyOrder, associatedServices, additionalServices);

            var isMultiFramework = IsMultiFramework();

            if (isMultiFramework)
            {
                TaskList.SelectFrameWork();
                SelectFundingSources.AddFundingSources(solutionName, isAssociatedServiceOnlyOrder, associatedServices, additionalServices);
            }
            else
            {
                var isLocalFundingOnly = IsLocalFundingOnly();
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

       // public void EditPlannedDeliveryDate(string solutionName, IEnumerable<string>? additionalServices, IEnumerable<string>? associatedServices, bool editplanneddeliverydate = true)
        public void EditPlannedDeliveryDate(string solutionName, string additionalService, string associatedService, bool editplanneddeliverydate = true)
        {
            //EditPlannedDeliveryDate(solutionName, new List<string> { additionalService }, new List<string> { associatedService }, editplanneddeliverydate);
            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnlyOrder();

            TaskList.EditPlannedDeliveryDateTask();
            PlannedDeliveryDates.EditPlannedDeliveryDateRout(solutionName, isAssociatedServiceOnlyOrder, additionalService, associatedService, editplanneddeliverydate);
           // PlannedDeliveryDates.EditPlannedDeliveryDateRout(solutionName, isAssociatedServiceOnlyOrder, additionalServices, associatedServices, editplanneddeliverydate);
        }

        public void StepThreeCompleteContract(bool isDefault = true)
        {
            TaskList.ImplementationPlanMilestonesTask();
            OrderingStepThree.SelectImplementationPlan(isDefault);

            using var dbContext = Factory.DbContext;
            var orderID = Driver.Url.Split('/').Last().Split('-')[0].Replace("C0", string.Empty);

            var isOrderWithAssociatedService = dbContext.Orders
                .Any(o => string.Equals(o.Id.ToString(), orderID) &&
                o.OrderItems.Any(i => i.CatalogueItem.CatalogueItemType == EntityFramework.Catalogue.Models.CatalogueItemType.AssociatedService));

            if (isOrderWithAssociatedService)
            {
                TaskList.AssociatedServiceBillingAndRequirementsTask();
                OrderingStepThree.SelectAssociatedServicesBilling(isDefault);
            }

            TaskList.DataProcessingInformationTask();
            OrderingStepThree.SelectPersonalDataProcessingInformation(isDefault);
        }

        public void StepFourReviewAndCompleteOrder()
        {
            TaskList.ReviewAndCompleteOrderTask();
            OrderingStepFour.ReviewAndCompleteOrder();
        }

        public void EditCatalogueSolution(string newSolutionName, string newAdditionalServiceName = "", string newAssociatedService = "", bool multipleServiceRecipients = false)
        {
           EditCatalogueSolution(newSolutionName, new List<string> { newAdditionalServiceName }, new List<string> { newAssociatedService }, multipleServiceRecipients);
        }

        public void EditCatalogueSolution(string newSolutionName, IEnumerable<string>? newAdditionalServiceNames, string newAssociatedService = "", bool multipleServiceRecipients = false)
        {
            EditCatalogueSolution(newSolutionName, newAdditionalServiceNames, new List<string> { newAssociatedService }, multipleServiceRecipients);
        }

        public void EditCatalogueSolution(string newSolutionName, string newAdditionalServiceName, IEnumerable<string>? newAssociatedServices, bool multipleServiceRecipients = false)
        {
            EditCatalogueSolution(newSolutionName, new List<string> { newAdditionalServiceName }, newAssociatedServices, multipleServiceRecipients);
        }

        public void EditCatalogueSolution(string newSolutionName, IEnumerable<string>? newAdditionalServiceNames, IEnumerable<string>? newAssociatedServices, bool multipleServiceRecipients = false)
        {
            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnlyOrder();

            TaskList.EditSolutionsAndServicesTask(isAssociatedServiceOnlyOrder);

            if (isAssociatedServiceOnlyOrder)
            {
                SelectEditAssociatedServiceOnly.EditSolutionForAssociatedService(newSolutionName, newAssociatedServices);

                if (newAssociatedServices != default && newAssociatedServices.All(a => !string.IsNullOrWhiteSpace(a)))
                {
                    foreach (var associatedService in newAssociatedServices)
                    {
                        SelectEditAssociatedServiceRecipientOnly.AddServiceRecipient(multipleServiceRecipients);
                        SelectEditAndConfirmAssociatedServiceOnlyPrices.SelectAndConfirmPrice();
                        Quantity.AddQuantity();
                    }
                }
            }
            else
            {
                SelectEditCatalogueSolution.EditSolution(newSolutionName, newAdditionalServiceNames);

                SelectEditCatalogueSolutionServiceRecipients.AddCatalogueSolutionServiceRecipient(multipleServiceRecipients);
                SelectEditAndConfirmPrices.SelectAndConfirmPrice();
                Quantity.AddQuantity();

                if (HasAdditionalService(newSolutionName) && newAdditionalServiceNames != default && newAdditionalServiceNames.All(a => !string.IsNullOrWhiteSpace(a)))
                {
                    foreach (var additionalService in newAdditionalServiceNames)
                    {
                        SelectEditAdditionalServiceRecipients.AddServiceRecipients();
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
            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnlyOrder();

            TaskList.EditSolutionsAndServicesTask(isAssociatedServiceOnlyOrder);

            SelectEditCatalogueSolution.EditAdditionalService(solutionName, oldAdditionalServices, newAdditionalServices, HasTheOriginalOrderAdditionalService());

            if (!HasAdditionalService(solutionName))
            {
                return;
            }

            if (HasAdditionalService(solutionName) && newAdditionalServices.All(a => !string.IsNullOrWhiteSpace(a)))
            {
                foreach (var additionalService in newAdditionalServices)
                {
                    SelectEditAdditionalServiceRecipients.AddServiceRecipients();
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
                    SelectEditAndConfirmAssociatedServicePrices.SelectAndConfirmPrice();
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
            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnlyOrder();

            TaskList.EditSolutionsAndServicesTask(isAssociatedServiceOnlyOrder);

            SelectEditAssociatedService.EditAssociatedService(solutionName, newAssociatedServices, HasTheOriginalOrderAssociatedService(), oldAssociatedServices);

            if (HasAssociatedServices(solutionName) && newAssociatedServices.All(a => !string.IsNullOrWhiteSpace(a)))
            {
                foreach (var associatedService in newAssociatedServices)
                {
                    SelectEditAssociatedServiceRecipents.AddServiceRecipient();
                    SelectEditAndConfirmAssociatedServicePrices.SelectAndConfirmPrice();
                    Quantity.AddQuantity();
                }
            }

            SolutionAndServicesReview.ReviewSolutionAndServices();

            TaskList.SelectFundingSourcesTask();
            SelectFundingSources.AddFundingSources(solutionName, isAssociatedServiceOnlyOrder, newAssociatedServices, additionalServices);
        }

        public void EditAssociatedServiceOnly(string solutionName, string newAssociatedServiceName, string oldAssociatedServiceName)
        {
            EditAssociatedServiceOnly(solutionName, new List<string> { newAssociatedServiceName }, new List<string> { oldAssociatedServiceName });
        }

        public void EditAssociatedServiceOnly(string solutionName, IEnumerable<string> newAssociatedServices, string oldAssociatedServiceName)
        {
            EditAssociatedServiceOnly(solutionName, newAssociatedServices, new List<string> { oldAssociatedServiceName });
        }

        public void EditAssociatedServiceOnly(string solutionName, IEnumerable<string> newAssociatedServices, IEnumerable<string> oldAssociatedServices)
        {
            var isAssociatedServiceOnlyOrder = IsAssociatedServiceOnlyOrder();

            TaskList.EditSolutionsAndServicesTask(isAssociatedServiceOnlyOrder);

            SelectEditAssociatedServiceOnly.EditAssociatedServiceOnly(newAssociatedServices, oldAssociatedServices);

            foreach (var associatedService in newAssociatedServices)
            {
                SelectEditAssociatedServiceRecipientOnly.AddServiceRecipient();
                SelectEditAndConfirmAssociatedServiceOnlyPrices.SelectAndConfirmPrice();
                Quantity.AddQuantity();
            }

            SolutionAndServicesReview.ReviewSolutionAndServices();

            TaskList.SelectFundingSourcesTask();
            SelectFundingSources.AddFundingSources(solutionName, isAssociatedServiceOnlyOrder, newAssociatedServices, null);
        }

        public void EditCatalogueSolutionServiceRecipient(string solutionName)
        {
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder());

            SelectEditCatalogueSolutionServiceRecipients.EditCatalogueSolutionServiceRecipient(solutionName);

            Quantity.EditQuantity(solutionName);
        }

        public void EditCatalogueSolutionPrice(string solutionName)
        {
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder());

            SelectEditAndConfirmPrices.EditCatalogueSolutionPrice(solutionName);
        }

        public void EditAdditionalServiceRecipient(string additionalServiceName)
        {
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder());

            SelectEditAdditionalServiceRecipients.EditAdditionalServiceRecipient(additionalServiceName);

            Quantity.EditQuantity(additionalServiceName);
        }

        public void EditAdditionalServicePrice(string additionalServiceName)
        {
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder());

            SelectEditAndConfirmAdditionalServicePrice.EditAdditionalServicePrice(additionalServiceName);
        }

        public void EditCatalogueItemQuantity(string catalogueItemName)
        {
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder());

            Quantity.EditQuantity(catalogueItemName);
        }

        public void EditAssociatedServiceRecipient(string associatedServiceName)
        {
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder());

            SelectEditAssociatedServiceRecipents.EditServiceRecipient(associatedServiceName);

            Quantity.EditQuantity(associatedServiceName);
        }

        public void EditAssociatedServicePrice(string associatedServiceName)
        {
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder());

            SelectEditAndConfirmAssociatedServicePrices.EditAssociatedServicePrice(associatedServiceName);
        }

        public void EditAssociatedServiceOnlyServiceRecipients(string associatedServiceName)
        {
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder());

            SelectEditAssociatedServiceRecipientOnly.EditServiceRecipient(associatedServiceName);

            Quantity.EditQuantity(associatedServiceName);
        }

        public void EditAssociatedServiceOnlyPrice(string associatedServiceName)
        {
            TaskList.EditSolutionsAndServicesTask(IsAssociatedServiceOnlyOrder());

            SelectEditAndConfirmAssociatedServiceOnlyPrices.EditPrice(associatedServiceName);
        }

        private bool IsAssociatedServiceOnlyOrder()
        {
            using var dbContext = Factory.DbContext;
            var orderID = Driver.Url.Split('/').Last().Split('-')[0].Replace("C0", string.Empty);

            return dbContext.Orders.Any(o => string.Equals(o.Id.ToString(), orderID) && o.AssociatedServicesOnly);
        }

        private bool IsMultiFramework()
        {
            using var dbContext = Factory.DbContext;
            var orderID = Driver.Url.Split('/').Last().Split('-')[0].Replace("C0", string.Empty);


            var frameworks = dbContext.OrderItems
             .Where(oi => string.Equals(oi.OrderId.ToString(), orderID))
             .SelectMany(oi => oi.CatalogueItem.Solution.FrameworkSolutions.Select(fs => fs.Framework)).ToList();

            return frameworks.Count() > 1;
        }

        private bool IsLocalFundingOnly()
        {
            using var dbContext = Factory.DbContext;
            var orderID = Driver.Url.Split('/').Last().Split('-')[0].Replace("C0", string.Empty);

            var frameworks = dbContext.OrderItems
             .Where(oi => string.Equals(oi.OrderId.ToString(), orderID))
             .SelectMany(oi => oi.CatalogueItem.Solution.FrameworkSolutions.Select(fs => fs.Framework)).ToList();

            if (frameworks.Count == 0 || frameworks.Count > 1)
            {
                return false;
            }

            var framework = frameworks.FirstOrDefault();
            return framework.LocalFundingOnly;
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
