using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class OrderEvent
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int EventId { get; set; }

        public Event Event { get; set; }
    }
}
