using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.TaskList.Base;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.TaskList
{
    public class TaskListNewOrder : TaskListBase
    {
        private const string InternalOrgId = "CG-03F";
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

        protected override bool SolutionDisplayed => true;

        protected override bool AdditionalServicesDisplayed => true;

        protected override List<TaskListOrderItem> OrderItems => new();

        protected override Type OnwardController => typeof(OrderController);

        protected override string OnwardAction => nameof(OrderController.Order);
    }
}
