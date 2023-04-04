using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.TaskList.Base;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.TaskList
{
    public class TaskListAmendedOrder : TaskListBase
    {
        private const string InternalOrgId = "CG-03F";
        private const int OrderId = 90031;
        private static readonly CallOffId CallOffId = new(OrderId, 2);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public TaskListAmendedOrder(LocalWebApplicationFactory factory)
            : base(factory, Parameters)
        {
        }

        protected override string PageTitle => $"Amend items from the original order - Order {CallOffId}";

        protected override bool AssociatedServicesDisplayed => false;

        protected override bool ChangeCatalogueSolutionLinkVisible => false;

        protected override bool ChangeAssociatedServicesLinkVisible => false;

        protected override List<TaskListOrderItem> OrderItems => new()
        {
            new TaskListOrderItem
            {
                Name = "E2E With Contact Multiple Prices",
                CatalogueItemId = new CatalogueItemId(99998, "001"),
                ServiceRecipientsAction = nameof(ServiceRecipientsController.EditServiceRecipients),
                PriceLinkActive = true,
                PriceAction = nameof(PricesController.ViewPrice),
                QuantityLinkActive = true,
                QuantityAction = nameof(QuantityController.ViewServiceRecipientQuantity),
            },
            new TaskListOrderItem
            {
                Name = "E2E Multiple Prices Additional Service",
                CatalogueItemId = new CatalogueItemId(99998, "001A99"),
                ServiceRecipientsAction = nameof(ServiceRecipientsController.AddServiceRecipients),
                PriceLinkActive = false,
                QuantityLinkActive = false,
            },
        };
    }
}
