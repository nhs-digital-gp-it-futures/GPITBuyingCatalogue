using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public sealed class AssociatedServiceModel : OrderingBaseModel
    {
        public AssociatedServiceModel(string internalOrgId, EntityFramework.Ordering.Models.Order order, List<OrderItem> orderItems)
        {
            Title = $"Associated Services for {order.CallOffId}";
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
