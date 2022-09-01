using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public class OrderSummaryItemModel
    {
        public OrderSummaryItemModel()
        {
        }

        public OrderSummaryItemModel(OrderItem orderItem)
        {
            OrderItem = orderItem;
        }

        public OrderItem OrderItem { get; set; }
    }
}
