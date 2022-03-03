using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public sealed class AdditionalServiceModel : OrderingBaseModel
    {
        public AdditionalServiceModel(string internalOrgId, EntityFramework.Ordering.Models.Order order, List<OrderItem> orderItems)
        {
            Title = $"Additional Services for {order.CallOffId}";
            InternalOrgId = internalOrgId;
            OrderDescription = order.Description;
            CallOffId = order.CallOffId;
            OrderItems = orderItems;
        }

        public string OrderDescription { get; set; }

        public CallOffId CallOffId { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}
