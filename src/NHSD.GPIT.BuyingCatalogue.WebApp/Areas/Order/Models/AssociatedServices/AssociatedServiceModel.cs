using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class AssociatedServiceModel : OrderingBaseModel
    {
        public AssociatedServiceModel(string odsCode, EntityFramework.Ordering.Models.Order order, List<OrderItem> orderItems)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{order.CallOffId}";
            BackLinkText = "Go back";
            Title = $"Associated Services for {order.CallOffId}";
            OdsCode = odsCode;
            OrderDescription = order.Description;
            CallOffId = order.CallOffId;
            OrderItems = orderItems;
        }

        public string OrderDescription { get; set; }

        public CallOffId CallOffId { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}
