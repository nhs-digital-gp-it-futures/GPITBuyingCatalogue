﻿using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.TaskList.Base;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Controllers.SolutionSelection;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.TaskList
{
    public class TaskListOrderWithSolution : TaskListBase
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90012;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public TaskListOrderWithSolution(LocalWebApplicationFactory factory)
            : base(factory, Parameters)
        {
        }

        protected override string PageTitle => "Review your progress - Order C090012-01";

        protected override bool SolutionDisplayed => true;

        protected override bool AdditionalServicesDisplayed => true;

        protected override List<TaskListOrderItem> OrderItems => new()
        {
            new TaskListOrderItem
            {
                Name = "E2E With Contact Multiple Prices",
                CatalogueItemId = new CatalogueItemId(99998, "001"),
                ServiceRecipientsAction = nameof(ServiceRecipientsController.AddServiceRecipients),
                PriceLinkActive = false,
                QuantityLinkActive = false,
            },
        };
    }
}
