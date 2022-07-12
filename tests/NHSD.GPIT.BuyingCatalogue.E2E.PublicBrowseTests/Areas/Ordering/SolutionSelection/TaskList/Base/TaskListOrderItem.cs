using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Areas.Ordering.SolutionSelection.TaskList.Base
{
    public class TaskListOrderItem
    {
        public CatalogueItemId CatalogueItemId { get; set; }

        public string Name { get; set; }

        public string ServiceRecipientsAction { get; set; }

        public bool PriceLinkActive { get; set; }

        public string PriceAction { get; set; }

        public bool QuantityLinkActive { get; set; }

        public string QuantityAction { get; set; }
    }
}
