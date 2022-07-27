using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList
{
    public sealed class TaskListService : ITaskListService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public TaskListService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public static void SetOrderTaskList(TaskListOrderSections completedSections, OrderTaskList model)
        {
            if (model is null)
                return;

            model.DescriptionStatus = TaskProgress.Completed;

            model.OrderingPartyStatus = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.OrderingPartyComplete) => TaskProgress.Completed,
                _ => TaskProgress.NotStarted,
            };

            model.SupplierStatus = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.SupplierContactComplete) => TaskProgress.Completed,
                var cs when cs.HasFlag(TaskListOrderSections.SupplierComplete) => TaskProgress.InProgress,
                var cs when cs.HasFlag(TaskListOrderSections.OrderingPartyComplete) => TaskProgress.NotStarted,
                _ => TaskProgress.CannotStart,
            };

            model.CommencementDateStatus = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.CommencementDateComplete) => TaskProgress.Completed,
                var cs when cs.HasFlag(TaskListOrderSections.SupplierContactComplete) => TaskProgress.NotStarted,
                _ => TaskProgress.CannotStart,
            };

            model.SolutionOrService = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.SolutionOrServiceComplete) => TaskProgress.Completed,
                var cs when cs.HasFlag(TaskListOrderSections.SolutionOrServiceInProgress) => TaskProgress.InProgress,
                var cs when cs.HasFlag(TaskListOrderSections.CommencementDateComplete) => TaskProgress.NotStarted,
                _ => TaskProgress.CannotStart,
            };

            model.FundingSource = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.FundingSourceComplete) => TaskProgress.Completed,
                var cs when cs.HasFlag(TaskListOrderSections.FundingSourceInProgress) => TaskProgress.InProgress,
                var cs when cs.HasFlag(TaskListOrderSections.SolutionOrServiceComplete) => TaskProgress.NotStarted,
                _ => TaskProgress.CannotStart,
            };

            model.ImplementationPlan = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.ImplementationPlanComplete) => TaskProgress.Completed,
                var cs when cs.HasFlag(TaskListOrderSections.ImplementationPlanInProgress) => TaskProgress.InProgress,
                var cs when cs.HasFlag(TaskListOrderSections.FundingSourceComplete) => TaskProgress.NotStarted,
                _ => TaskProgress.CannotStart,
            };

            model.AssociatedServiceBilling = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.AssociatedServiceBillingNotApplicable) => TaskProgress.NotApplicable,
                var cs when cs.HasFlag(TaskListOrderSections.AssociatedServiceBillingComplete) => TaskProgress.Completed,
                var cs when cs.HasFlag(TaskListOrderSections.AssociatedServiceBillingInProgress) => TaskProgress.InProgress,
                var cs when cs.HasFlag(TaskListOrderSections.ImplementationPlanComplete) => TaskProgress.NotStarted,
                _ => TaskProgress.CannotStart,
            };

            model.DataProcessingInformation = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.DataProcessingInformationCompleted)
                    && (cs.HasFlag(TaskListOrderSections.AssociatedServiceBillingComplete)
                        || cs.HasFlag(TaskListOrderSections.AssociatedServiceBillingNotApplicable)) => TaskProgress.Completed,
                var cs when (
                        cs.HasFlag(TaskListOrderSections.AssociatedServiceBillingNotApplicable)
                        && cs.HasFlag(TaskListOrderSections.ImplementationPlanComplete))
                    || (!cs.HasFlag(TaskListOrderSections.AssociatedServiceBillingNotApplicable)
                        && cs.HasFlag(TaskListOrderSections.AssociatedServiceBillingComplete))
                    => TaskProgress.NotStarted,
                _ => TaskProgress.CannotStart,
            };

            model.ReviewAndCompleteStatus = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.DataProcessingInformationCompleted) => TaskProgress.NotStarted,
                _ => TaskProgress.CannotStart,
            };
        }

        public async Task<OrderTaskList> GetTaskListStatusModelForOrder(int? orderId)
        {
            var model = new OrderTaskList();

            if (orderId is null)
                return model;

            var completedSections = await GetOrderSectionFlags(orderId.Value);
            var sectionStatuses = SetOrderSectionFlags(completedSections);
            SetOrderTaskList(sectionStatuses, model);

            return model;
        }

        private static TaskListOrderSections SetOrderSectionFlags(OrderTaskListCompletedSections orderStatuses)
        {
            var completedSections = TaskListOrderSections.Description;

            if (orderStatuses is null)
                return completedSections;

            if (orderStatuses.OrderContactDetailsCompleted)
                completedSections |= TaskListOrderSections.OrderingParty;

            if (orderStatuses.SupplierSelected)
                completedSections |= TaskListOrderSections.Supplier;

            if (orderStatuses.SupplierContactSelected)
                completedSections |= TaskListOrderSections.SupplierContact;

            if (orderStatuses.TimeScalesCompleted)
                completedSections |= TaskListOrderSections.CommencementDate;

            if (orderStatuses.SolutionsSelected)
                completedSections |= TaskListOrderSections.SolutionOrServiceInProgress;

            if (orderStatuses.SolutionsCompleted)
                completedSections |= TaskListOrderSections.SolutionOrService;

            if (orderStatuses.FundingInProgress)
                completedSections |= TaskListOrderSections.FundingSourceInProgress;

            if (orderStatuses.FundingCompleted)
                completedSections |= TaskListOrderSections.FundingSource;

            if (!orderStatuses.HasAssociatedServices)
                completedSections |= TaskListOrderSections.AssociatedServiceBillingNotApplicable;

            if (orderStatuses.HasImplementationPlan)
                completedSections |= TaskListOrderSections.ImplementationPlanComplete;

            if (orderStatuses.DataProcessingPlanCompleted)
                completedSections |= TaskListOrderSections.DataProcessingInformation;

            return completedSections;
        }

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

        private async Task<OrderTaskListCompletedSections> GetOrderSectionFlags(int orderId)
        {
            var order = await dbContext.Orders
                .Include(x => x.OrderItems).ThenInclude(x => x.CatalogueItem)
                .Include(x => x.OrderItems).ThenInclude(x => x.OrderItemFunding)
                .Include(x => x.OrderItems).ThenInclude(x => x.OrderItemPrice)
                .Include(x => x.OrderItems).ThenInclude(x => x.OrderItemRecipients)
                .Include(x => x.OrderingPartyContact)
                .Include(x => x.Supplier)
                .Include(x => x.SupplierContact)
                .AsSplitQuery()
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.Id == orderId);

            if (order == null)
            {
                return null;
            }

            var contract = await dbContext.ContractFlags.SingleOrDefaultAsync(x => x.OrderId == orderId);

            return new OrderTaskListCompletedSections
            {
                OrderContactDetailsCompleted = order.OrderingPartyContact != null,
                SupplierSelected = order.Supplier != null,
                SupplierContactSelected = order.SupplierContact != null,
                TimeScalesCompleted = order.CommencementDate != null,
                SolutionsSelected = SolutionsSelected(order),
                SolutionsCompleted = SolutionsCompleted(order),
                HasAssociatedServices = order.OrderItems.Any(oi => oi.CatalogueItem.CatalogueItemType is CatalogueItemType.AssociatedService),
                FundingInProgress = order.OrderItems.Any(oi => oi.OrderItemFunding != null),
                FundingCompleted = order.OrderItems.All(oi => oi.OrderItemFunding != null),
                HasImplementationPlan = contract?.UseDefaultImplementationPlan != null,
                DataProcessingPlanCompleted = contract?.UseDefaultDataProcessing != null,
                OrderCompleted = order.Completed != null,
            };
        }
    }
}
