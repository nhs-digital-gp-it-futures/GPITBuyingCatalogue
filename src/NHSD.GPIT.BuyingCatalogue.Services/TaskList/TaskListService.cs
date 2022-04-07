using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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

            model.ReviewAndCompleteStatus = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.FundingSourceComplete) => TaskProgress.NotStarted,
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

        public async Task<OrderTaskListCompletedSections> GetOrderSectionFlags(int orderId) =>
            await dbContext.Orders.AsNoTracking()
                 .Where(o => o.Id == orderId)
                 .Select(o => new OrderTaskListCompletedSections
                 {
                     OrderContactDetailsCompleted = o.OrderingPartyContact != null,
                     SupplierSelected = o.Supplier != null,
                     SupplierContactSelected = o.SupplierContact != null,
                     TimeScalesCompleted = o.CommencementDate != null,
                     SolutionsSelected = o.OrderItems.Any(),
                     SolutionsCompleted = o.OrderItems.All(oi => oi.CatalogueItem != null && oi.OrderItemPrice != null && oi.OrderItemRecipients != null && oi.OrderItemRecipients.All(oir => oir.Quantity > 0)),
                     FundingInProgress = // true if any associated services or non-locally funded solutions have a funding but not all
                        o.OrderItems.Where(oi =>
                             oi.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService
                             || (oi.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution && oi.CatalogueItem.Solution.FrameworkSolutions.Any(fs => !fs.Framework.LocalFundingOnly)))
                             .Any(oi => oi.OrderItemFunding != null),
                     FundingCompleted = o.OrderItems.All(oi => oi.OrderItemFunding != null),
                     OrderCompleted = o.Completed != null,
                 }).SingleOrDefaultAsync();

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

            return completedSections;
        }
    }
}
