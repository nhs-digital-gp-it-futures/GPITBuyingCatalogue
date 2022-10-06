using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public struct OrderTaskList
    {
        public OrderTaskList()
        {
        }

        public OrderTaskList(
            EntityFramework.Ordering.Models.Order order)
        {
            SetStatusFlags(order);
        }

        public TaskProgress DescriptionStatus { get; set; } = TaskProgress.NotStarted;

        public TaskProgress OrderingPartyStatus { get; set; } = TaskProgress.CannotStart;

        public TaskProgress SupplierStatus { get; set; } = TaskProgress.CannotStart;

        public TaskProgress CommencementDateStatus { get; set; } = TaskProgress.CannotStart;

        public TaskProgress SolutionOrService { get; set; } = TaskProgress.CannotStart;

        public TaskProgress DeliveryDates { get; set; } = TaskProgress.CannotStart;

        public TaskProgress FundingSource { get; set; } = TaskProgress.CannotStart;

        public TaskProgress ImplementationPlan { get; set; } = TaskProgress.CannotStart;

        public TaskProgress AssociatedServiceBilling { get; set; } = TaskProgress.CannotStart;

        public TaskProgress DataProcessingInformation { get; set; } = TaskProgress.CannotStart;

        public TaskProgress ReviewAndCompleteStatus { get; set; } = TaskProgress.CannotStart;

        private static bool HasAssociatedServices(EntityFramework.Ordering.Models.Order order) =>
            order.AssociatedServicesOnly
            || order.HasAssociatedService();

        private static bool SolutionsSelected(EntityFramework.Ordering.Models.Order order)
        {
            if (order.AssociatedServicesOnly)
            {
                return order.SolutionId != null;
            }

            return order.OrderItems.Any(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution);
        }

        private static bool SolutionsCompleted(EntityFramework.Ordering.Models.Order order)
        {
            if (!order.OrderItems.Any())
            {
                return false;
            }

            return order.OrderItems.All(x =>
                x.CatalogueItem != null
                && x.OrderItemPrice != null
                && (x.OrderItemRecipients?.Any() ?? false)
                && x.AllQuantitiesEntered);
        }

        private void SetStatusFlags(EntityFramework.Ordering.Models.Order order)
        {
            SetSectionOneStatus(order);
            SetSectionTwoStatus(order);
            SetSectionThreeStatus(order);
            SetSectionFourStatus(order);
        }

        private void SetSectionOneStatus(EntityFramework.Ordering.Models.Order order)
        {
            DescriptionStatus = TaskProgress.Completed;

            OrderingPartyStatus =
                order.OrderingPartyContact != null ? TaskProgress.Completed : TaskProgress.NotStarted;

            if (OrderingPartyStatus is not TaskProgress.Completed)
                return;

            if (order.Supplier != null && order.SupplierContact != null)
                SupplierStatus = TaskProgress.Completed;
            else if (order.Supplier != null && order.SupplierContact == null)
                SupplierStatus = TaskProgress.InProgress;
            else
                SupplierStatus = TaskProgress.NotStarted;

            if (SupplierStatus is not TaskProgress.Completed)
                return;

            CommencementDateStatus = order.CommencementDate != null ? TaskProgress.Completed : TaskProgress.NotStarted;
        }

        private void SetSectionTwoStatus(EntityFramework.Ordering.Models.Order order)
        {
            if (CommencementDateStatus is not TaskProgress.Completed)
                return;

            if (!SolutionsSelected(order))
                SolutionOrService = TaskProgress.NotStarted;
            else if (SolutionsSelected(order) && !SolutionsCompleted(order))
                SolutionOrService = TaskProgress.InProgress;
            else
                SolutionOrService = TaskProgress.Completed;

            var allDeliveryDatesEntered = order.OrderItems
                .SelectMany(x => x.OrderItemRecipients)
                .All(x => x.DeliveryDate != null);

            var anyDeliveryDatesEntered = order.OrderItems
                .SelectMany(x => x.OrderItemRecipients)
                .Any(x => x.DeliveryDate != null);

            // Planned Delivery Dates
            if (SolutionOrService is TaskProgress.Completed
                || anyDeliveryDatesEntered)
            {
                DeliveryDates = allDeliveryDatesEntered
                    ? TaskProgress.Completed
                    : (anyDeliveryDatesEntered ? TaskProgress.InProgress : TaskProgress.NotStarted);
            }

            FundingSource = DeliveryDates switch
            {
                TaskProgress.CannotStart => TaskProgress.CannotStart,
                TaskProgress.NotStarted => TaskProgress.CannotStart,
                TaskProgress.InProgress when order.OrderItems.All(oi => oi.OrderItemFunding == null) => TaskProgress.CannotStart,
                TaskProgress.Completed when order.OrderItems.All(oi => oi.OrderItemFunding != null) => TaskProgress.Completed,
                _ => order.OrderItems.Any(oi => oi.OrderItemFunding != null)
                    ? TaskProgress.InProgress
                    : TaskProgress.NotStarted,
            };
        }

        private void SetSectionThreeStatus(EntityFramework.Ordering.Models.Order order)
        {
            // Default Implementation Plan
            if (FundingSource is not TaskProgress.Completed
                && order.ContractFlags?.UseDefaultImplementationPlan != null)
            {
                ImplementationPlan = TaskProgress.InProgress;
            }
            else if (FundingSource is TaskProgress.Completed)
            {
                ImplementationPlan = order.ContractFlags?.UseDefaultImplementationPlan != null
                    ? TaskProgress.Completed
                    : TaskProgress.NotStarted;
            }

            // Associated Services Billing
            if ((SolutionOrService == TaskProgress.InProgress
                    || ImplementationPlan == TaskProgress.InProgress)
                && (order.ContractFlags?.HasSpecificRequirements != null
                    || order.ContractFlags?.UseDefaultBilling != null))
            {
                AssociatedServiceBilling = TaskProgress.InProgress;
            }
            else if (SolutionOrService != TaskProgress.Completed)
            {
                AssociatedServiceBilling = TaskProgress.CannotStart;
            }
            else if (!HasAssociatedServices(order))
            {
                AssociatedServiceBilling = TaskProgress.NotApplicable;
            }
            else if (ImplementationPlan != TaskProgress.Completed)
            {
                AssociatedServiceBilling = TaskProgress.CannotStart;
            }
            else
            {
                AssociatedServiceBilling = order.ContractFlags?.HasSpecificRequirements switch
                {
                    null when order.ContractFlags?.UseDefaultBilling is null => TaskProgress.NotStarted,
                    null => TaskProgress.InProgress,
                    _ => TaskProgress.Completed,
                };
            }

            // Data Processing
            if ((FundingSource is not TaskProgress.Completed
                    || (AssociatedServiceBilling is not TaskProgress.Completed
                        && AssociatedServiceBilling is not TaskProgress.NotApplicable))
                && order.ContractFlags?.UseDefaultDataProcessing != null)
            {
                DataProcessingInformation = TaskProgress.InProgress;
            }
            else
            if ((AssociatedServiceBilling is TaskProgress.Completed)
                || (AssociatedServiceBilling is TaskProgress.NotApplicable
                    && ImplementationPlan is TaskProgress.Completed))
            {
                DataProcessingInformation = order.ContractFlags?.UseDefaultDataProcessing != null
                    ? TaskProgress.Completed
                    : TaskProgress.NotStarted;
            }
        }

        private void SetSectionFourStatus(EntityFramework.Ordering.Models.Order order)
        {
            if (order.Completed != null)
                ReviewAndCompleteStatus = TaskProgress.Completed;
            else if (DataProcessingInformation is TaskProgress.Completed)
                ReviewAndCompleteStatus = TaskProgress.NotStarted;
            else
                ReviewAndCompleteStatus = TaskProgress.CannotStart;
        }
    }
}
