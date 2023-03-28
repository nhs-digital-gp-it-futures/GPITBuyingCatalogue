using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.TaskList.Base;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Controllers.SolutionSelection;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.TaskList
{
    public class TaskListOrderWithSolutionAndAdditionalService : TaskListBase
    {
        private const string InternalOrgId = "IB-QWO";
        private const int OrderId = 91007;
        private static readonly CallOffId CallOffId = new(OrderId, 1);

        private static readonly Dictionary<string, string> Parameters = new()
        {
            { nameof(InternalOrgId), InternalOrgId },
            { nameof(CallOffId), $"{CallOffId}" },
        };

        public TaskListOrderWithSolutionAndAdditionalService(LocalWebApplicationFactory factory)
            : base(factory, Parameters)
        {
        }

        protected override string PageTitle => "Review your progress - Order C091007-01";

        protected override List<TaskListOrderItem> OrderItems => new()
        {
            new TaskListOrderItem
            {
                Name = "E2E With Contact Multiple Prices",
                CatalogueItemId = new CatalogueItemId(99998, "001"),
                ServiceRecipientsAction = nameof(ServiceRecipientsController.EditServiceRecipients),
                PriceLinkActive = true,
                PriceAction = nameof(PricesController.SelectPrice),
                QuantityLinkActive = true,
                QuantityAction = nameof(QuantityController.SelectServiceRecipientQuantity),
            },
            new TaskListOrderItem
            {
                Name = "E2E No Contact Single Price Additional Service",
                CatalogueItemId = new CatalogueItemId(99998, "002A999"),
                ServiceRecipientsAction = nameof(ServiceRecipientsController.EditServiceRecipients),
                PriceLinkActive = true,
                PriceAction = nameof(PricesController.EditPrice),
                QuantityLinkActive = true,
                QuantityAction = nameof(QuantityController.SelectQuantity),
            },
        };
    }
}
