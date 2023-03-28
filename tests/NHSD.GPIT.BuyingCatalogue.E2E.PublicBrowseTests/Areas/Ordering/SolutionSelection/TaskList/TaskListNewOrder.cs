using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.TaskList.Base;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.TaskList
{
    public class TaskListNewOrder : TaskListBase
    {
        private const string InternalOrgId = "IB-QWO";
        private const int OrderId = 90000;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public TaskListNewOrder(LocalWebApplicationFactory factory)
            : base(factory, Parameters)
        {
        }

        protected override string PageTitle => "Edit solutions and services - Order C090000-01";

        protected override bool ChangeAdditionalServicesLinkVisible => false;

        protected override bool ChangeAssociatedServicesLinkVisible => false;

        protected override List<TaskListOrderItem> OrderItems => new();
    }
}
