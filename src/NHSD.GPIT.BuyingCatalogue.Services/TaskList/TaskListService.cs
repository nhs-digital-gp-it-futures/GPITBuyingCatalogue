using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;
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
                var cs when cs.HasFlag(TaskListOrderSections.CommencementDateComplete) => TaskProgress.NotStarted,
                _ => TaskProgress.CannotStart,
            };

            model.FundingSource = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.SolutionOrServiceComplete) => TaskProgress.Completed,
                var cs when cs.HasFlag(TaskListOrderSections.FundingSourceInProgress) => TaskProgress.InProgress,
                var cs when cs.HasFlag(TaskListOrderSections.SolutionOrServiceComplete) => TaskProgress.NotStarted,
                _ => TaskProgress.CannotStart,
            };

            model.ReviewAndCompleteStatus = completedSections switch
            {
                var cs when cs.HasFlag(TaskListOrderSections.FundingSourceComplete) => TaskProgress.NotStarted,
                _ => TaskProgress.CannotStart,
            };

            return model;
        }

        private static TaskListOrderSections SetOrderSectionFlags(Order order)
        {
            var completedSections = TaskListOrderSections.Description;

            if (order.OrderingPartyContact is not null)
                completedSections |= TaskListOrderSections.OrderingParty;

            if (order.Supplier is not null)
                completedSections |= TaskListOrderSections.Supplier;

            if (order.SupplierContactId is not null)
                completedSections |= TaskListOrderSections.SupplierContact;

            if (order.CommencementDate is not null)
                completedSections |= TaskListOrderSections.CommencementDate;

            if (order.HasSolution() || order.HasAssociatedService())
                completedSections |= TaskListOrderSections.SolutionOrService;

            if (order.OrderItems.Any(oi => oi.OrderItemFunding is not null) && !order.OrderItems.All(oi => oi.OrderItemFunding is not null))
                completedSections |= TaskListOrderSections.FundingSourceInProgress;

            if (order.OrderItems.All(oi => oi.OrderItemFunding is not null))
                completedSections |= TaskListOrderSections.FundingSource;

            return completedSections;
        }
    }
}
