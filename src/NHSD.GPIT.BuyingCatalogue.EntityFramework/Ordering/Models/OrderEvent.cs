using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderEvent
    {
        public int Id { get; set; }

        public int? OrderNumber { get; set; }

        public int? OrderId { get; set; }

        public int EventTypeId { get; set; }

        public EventType EventType { get; set; }
    }
}
