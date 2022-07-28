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

        public TaskProgress FundingSource { get; set; } = TaskProgress.CannotStart;

        public TaskProgress ImplementationPlan { get; set; } = TaskProgress.CannotStart;

        public TaskProgress AssociatedServiceBilling { get; set; } = TaskProgress.CannotStart;

        public TaskProgress DataProcessingInformation { get; set; } = TaskProgress.CannotStart;

        public TaskProgress ReviewAndCompleteStatus { get; set; } = TaskProgress.CannotStart;

        private static bool HasAssociatedServices(EntityFramework.Ordering.Models.Order order) =>
            order.AssociatedServicesOnly
            || order.OrderItems.Any(
                oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService);

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
            SetSectionFourStatus();
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

            if (SolutionOrService is not TaskProgress.Completed)
                return;

            if (order.OrderItems.All(oi => oi.OrderItemFunding != null))
                FundingSource = TaskProgress.Completed;
            else if (order.OrderItems.All(oi => oi.OrderItemFunding == null))
                FundingSource = TaskProgress.NotStarted;
            else
                FundingSource = TaskProgress.InProgress;
        }

        private void SetSectionThreeStatus(EntityFramework.Ordering.Models.Order order)
        {
            var hasAssociatedServices = HasAssociatedServices(order);

            if (FundingSource is TaskProgress.Completed)
            {
                ImplementationPlan = order.ContractFlags?.UseDefaultImplementationPlan != null
                    ? TaskProgress.Completed
                    : TaskProgress.NotStarted;
            }

            if (!hasAssociatedServices)
            {
                AssociatedServiceBilling = TaskProgress.NotApplicable;
            }
            else if (ImplementationPlan is TaskProgress.Completed)
            {
                AssociatedServiceBilling = order.ContractFlags?.UseDefaultBilling != null
                    ? TaskProgress.Completed
                    : TaskProgress.NotStarted;
            }

            if ((AssociatedServiceBilling is TaskProgress.Completed)
                || (AssociatedServiceBilling is TaskProgress.NotApplicable
                    && ImplementationPlan is TaskProgress.Completed))
            {
                DataProcessingInformation = order.ContractFlags?.UseDefaultDataProcessing != null
                    ? TaskProgress.Completed
                    : TaskProgress.NotStarted;
            }
        }

        private void SetSectionFourStatus()
        {
            ReviewAndCompleteStatus = DataProcessingInformation is TaskProgress.Completed
                ? TaskProgress.NotStarted
                : TaskProgress.CannotStart;
        }
    }
}
