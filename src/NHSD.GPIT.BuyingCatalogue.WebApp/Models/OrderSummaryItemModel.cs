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

        public OrderSummaryItemModel(OrderItem orderItem, IEnumerable<ServiceInstanceItem> serviceInstanceItems)
        {
            OrderItem = orderItem;
            ServiceInstanceItems = serviceInstanceItems;
        }

        public OrderItem OrderItem { get; set; }

        public IEnumerable<ServiceInstanceItem> ServiceInstanceItems { get; set; }

        public string ServiceInstanceId(string odsCode) => ServiceInstanceItems.FirstOrDefault(x => x.OdsCode == odsCode)?.ServiceInstanceId;
    }
}
