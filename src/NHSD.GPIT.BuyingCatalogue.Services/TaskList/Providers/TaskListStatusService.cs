using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.Services.TaskList.Providers;

public static class TaskListStatusService
{
    private static readonly TaskProgress[] TaskCompleteStatuses = [TaskProgress.Completed, TaskProgress.Amended];

    public static bool IsTaskCompleted(TaskProgress taskStatus) =>
        TaskCompleteStatuses.Contains(taskStatus);

    public static TaskProgress CompletedOrAmended(bool isAmendment) =>
        isAmendment ? TaskProgress.Amended : TaskProgress.Completed;

    public static bool HasAssociatedServices(Order order) =>
        order == null
        ? throw new ArgumentNullException(nameof(order))
        : order.OrderType.AssociatedServicesOnly || order.HasAssociatedService();
}
