using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.TaskList;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.TaskList;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList
{
    public sealed class TaskListService : ITaskListService
    {
        public OrderTaskList GetTaskListStatusModelForOrder(Order order)
        {
            var model = new OrderTaskList();

            if (order is null)
                return model;

            var completedSections = SetOrderSectionFlags(order);

            model.DescriptionStatus = TaskListStatuses.Completed;

            model.OrderingPartyStatus = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.OrderingPartyComplete) => TaskListStatuses.Completed,
                _ => TaskListStatuses.Incomplete,
            };

            model.SupplierStatus = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.SupplierComplete) => TaskListStatuses.Completed,
                var cs when cs.HasFlag(TaskListOrderSections.OrderingPartyComplete) => TaskListStatuses.Incomplete,
                _ => TaskListStatuses.CannotStart,
            };

            model.CommencementDateStatus = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.CommencementDateComplete) => TaskListStatuses.Completed,
                var cs when cs.HasFlag(TaskListOrderSections.SupplierComplete) => TaskListStatuses.Incomplete,
                _ => TaskListStatuses.CannotStart,
            };

            model.CatalogueSolutionsStatus = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.CatalogueSolutionsComplete) => TaskListStatuses.Completed,
                var cs when cs.HasFlag(TaskListOrderSections.AssociatedServicesComplete) => TaskListStatuses.Optional,
                var cs when cs.HasFlag(TaskListOrderSections.CommencementDateComplete) => TaskListStatuses.Incomplete,
                _ => TaskListStatuses.CannotStart,
            };

            model.AdditionalServiceStatus = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.AdditionalServicesComplete) => TaskListStatuses.Completed,
                var cs when cs.HasFlag(TaskListOrderSections.CatalogueSolutionsComplete) => TaskListStatuses.Optional,
                _ => TaskListStatuses.CannotStart,
            };

            model.AssociatedServiceStatus = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.AssociatedServicesComplete) => TaskListStatuses.Completed,
                var cs when cs.HasFlag(TaskListOrderSections.CatalogueSolutionsComplete) => TaskListStatuses.Optional,
                var cs when cs.HasFlag(TaskListOrderSections.CommencementDateComplete) => TaskListStatuses.Incomplete,
                _ => TaskListStatuses.CannotStart,
            };

            model.FundingSourceStatus = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.FundingSourceComplete) => TaskListStatuses.Completed,
                var cs when
                cs.HasFlag(TaskListOrderSections.CatalogueSolutionsComplete)
                || cs.HasFlag(TaskListOrderSections.AssociatedServicesComplete) => TaskListStatuses.Incomplete,
                _ => TaskListStatuses.CannotStart,
            };

            model.OrderCompletable =
                (completedSections.HasFlag(TaskListOrderSections.FundingSourceComplete)
                && completedSections.HasFlag(TaskListOrderSections.CatalogueSolutionsComplete))
                || completedSections.HasFlag(TaskListOrderSections.AssociatedServicesComplete);

            model.OrderComplete = order.Completed.HasValue;

            return model;
        }

        private static TaskListOrderSections SetOrderSectionFlags(Order order)
        {
            var completedSections = TaskListOrderSections.Description;

            if (order.OrderingPartyContact is not null)
                completedSections |= TaskListOrderSections.OrderingParty;

            if (order.Supplier is not null)
                completedSections |= TaskListOrderSections.Supplier;

            if (order.CommencementDate is not null)
                completedSections |= TaskListOrderSections.CommencementDate;

            if (order.HasSolution())
                completedSections |= TaskListOrderSections.CatalogueSolutions;

            if (order.OrderItems.Any(oi => oi.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService))
                completedSections |= TaskListOrderSections.AdditionalServices;

            if (order.HasAssociatedService())
                completedSections |= TaskListOrderSections.AssociatedServices;

            if (order.FundingSourceOnlyGms.HasValue)
                completedSections |= TaskListOrderSections.FundingSource;

            return completedSections;
        }
    }
}
