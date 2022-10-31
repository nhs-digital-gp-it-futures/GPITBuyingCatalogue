using System;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders
{
    public class OrderTaskListService : IOrderTaskListService
    {
        private readonly IOrderProgressService orderProgressService;
        private OrderWrapper wrapper;

        public OrderTaskListService(IOrderProgressService orderProgressService)
        {
            this.orderProgressService = orderProgressService ?? throw new ArgumentNullException(nameof(orderProgressService));
        }

        public OrderTaskListModel GetTaskListStatuses(OrderWrapper orderWrapper)
        {
            wrapper = orderWrapper ?? throw new ArgumentNullException(nameof(orderWrapper));

            var output = new OrderTaskListModel();

            SetStatusFlags(output, wrapper.Order);

            return output;
        }

        private static bool HasAssociatedServices(Order order) =>
            order.AssociatedServicesOnly
            || order.HasAssociatedService();

        private static bool SolutionsSelected(Order order)
        {
            if (order.AssociatedServicesOnly)
            {
                return order.SolutionId != null;
            }

            return order.OrderItems.Any(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution);
        }

        private static bool SolutionsCompleted(Order order)
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

        private static bool SomeSolutionsCompleted(Order order)
        {
            if (!order.OrderItems.Any())
            {
                return false;
            }

            return order.OrderItems.Any(x =>
                x.CatalogueItem != null
                && x.OrderItemPrice != null
                && (x.OrderItemRecipients?.Any() ?? false)
                && x.AllQuantitiesEntered);
        }

        private static bool AllDeliveryDatesEntered(Order order)
        {
            var recipients = order.OrderItems
                .SelectMany(x => x.OrderItemRecipients)
                .ToList();

            return recipients.Any()
                && recipients.All(x => x.DeliveryDate != null);
        }

        private static bool AnyDeliveryDatesEntered(Order order)
        {
            return order.OrderItems
                .SelectMany(x => x.OrderItemRecipients)
                .Any(x => x.DeliveryDate != null);
        }

        private static bool AllFundingSourcesEntered(Order order)
        {
            return order.SelectedFramework != null
                && order.OrderItems.Any()
                && order.OrderItems.All(x => x.OrderItemFunding != null);
        }

        private static bool AnyFundingSourcesEntered(Order order)
        {
            return order.OrderItems.Any(x => x.OrderItemFunding != null);
        }

        private void SetStatusFlags(OrderTaskListModel model, Order order)
        {
            SetSectionOneStatus(model, order);
            SetSectionTwoStatus(model, order);
            SetSectionThreeStatus(model, order);
            SetSectionFourStatus(model, order);
        }

        private void SetSectionOneStatus(OrderTaskListModel model, Order order)
        {
            model.DescriptionStatus = orderProgressService.DescriptionStatus(wrapper);

            model.OrderingPartyStatus = order.OrderingPartyContact != null
                ? TaskProgress.Completed
                : TaskProgress.NotStarted;

            if (model.OrderingPartyStatus is not TaskProgress.Completed)
                return;

            if (order.Supplier != null && order.SupplierContact != null)
                model.SupplierStatus = TaskProgress.Completed;
            else if (order.Supplier != null && order.SupplierContact == null)
                model.SupplierStatus = TaskProgress.InProgress;
            else
                model.SupplierStatus = TaskProgress.NotStarted;

            if (model.SupplierStatus is not TaskProgress.Completed)
                return;

            model.CommencementDateStatus = order.CommencementDate != null ? TaskProgress.Completed : TaskProgress.NotStarted;
        }

        private void SetSectionTwoStatus(OrderTaskListModel model, Order order)
        {
            if (model.CommencementDateStatus is not TaskProgress.Completed)
                return;

            if (!SolutionsSelected(order))
                model.SolutionOrService = TaskProgress.NotStarted;
            else if (SolutionsSelected(order) && !SolutionsCompleted(order))
                model.SolutionOrService = TaskProgress.InProgress;
            else
                model.SolutionOrService = TaskProgress.Completed;

            if (model.SolutionOrService is TaskProgress.Completed
                || AnyDeliveryDatesEntered(order))
            {
                model.DeliveryDates = AllDeliveryDatesEntered(order)
                    ? TaskProgress.Completed
                    : (AnyDeliveryDatesEntered(order) ? TaskProgress.InProgress : TaskProgress.NotStarted);
            }

            if (model.DeliveryDates != TaskProgress.Completed
                && !AnyFundingSourcesEntered(order))
            {
                return;
            }

            model.FundingSource = AllFundingSourcesEntered(order)
                ? TaskProgress.Completed
                : (AnyFundingSourcesEntered(order) ? TaskProgress.InProgress : TaskProgress.NotStarted);
        }

        private void SetSectionThreeStatus(OrderTaskListModel model, Order order)
        {
            // Default Implementation Plan
            if (model.FundingSource is not TaskProgress.Completed
                && order.ContractFlags?.UseDefaultImplementationPlan != null)
            {
                model.ImplementationPlan = TaskProgress.InProgress;
            }
            else if (model.FundingSource is TaskProgress.Completed)
            {
                model.ImplementationPlan = order.ContractFlags?.UseDefaultImplementationPlan != null
                    ? TaskProgress.Completed
                    : TaskProgress.NotStarted;
            }

            // Associated Services Billing
            if (!HasAssociatedServices(order))
            {
                model.AssociatedServiceBilling = TaskProgress.NotApplicable;
            }
            else
            {
                if ((model.FundingSource == TaskProgress.InProgress || model.ImplementationPlan == TaskProgress.InProgress)
                    && (order.ContractFlags?.HasSpecificRequirements != null || order.ContractFlags?.UseDefaultBilling != null))
                {
                    model.AssociatedServiceBilling = TaskProgress.InProgress;
                }
                else if (model.FundingSource != TaskProgress.Completed)
                {
                    model.AssociatedServiceBilling = TaskProgress.CannotStart;
                }
                else if (model.ImplementationPlan != TaskProgress.Completed)
                {
                    model.AssociatedServiceBilling = TaskProgress.CannotStart;
                }
                else
                {
                    model.AssociatedServiceBilling = order.ContractFlags?.HasSpecificRequirements switch
                    {
                        null when order.ContractFlags?.UseDefaultBilling is null => TaskProgress.NotStarted,
                        null => TaskProgress.InProgress,
                        _ => TaskProgress.Completed,
                    };
                }
            }

            // Data Processing
            if ((model.FundingSource is not TaskProgress.Completed
                    || (model.AssociatedServiceBilling is not TaskProgress.Completed && model.AssociatedServiceBilling is not TaskProgress.NotApplicable))
                && order.ContractFlags?.UseDefaultDataProcessing != null)
            {
                model.DataProcessingInformation = TaskProgress.InProgress;
            }
            else
            if ((model.AssociatedServiceBilling is TaskProgress.Completed)
                || (model.AssociatedServiceBilling is TaskProgress.NotApplicable && model.ImplementationPlan is TaskProgress.Completed))
            {
                model.DataProcessingInformation = order.ContractFlags?.UseDefaultDataProcessing != null
                    ? TaskProgress.Completed
                    : TaskProgress.NotStarted;
            }
        }

        private void SetSectionFourStatus(OrderTaskListModel model, Order order)
        {
            if (order.Completed != null)
                model.ReviewAndCompleteStatus = TaskProgress.Completed;
            else if (model.DataProcessingInformation is TaskProgress.Completed)
                model.ReviewAndCompleteStatus = TaskProgress.NotStarted;
            else
                model.ReviewAndCompleteStatus = TaskProgress.CannotStart;
        }
    }
}
