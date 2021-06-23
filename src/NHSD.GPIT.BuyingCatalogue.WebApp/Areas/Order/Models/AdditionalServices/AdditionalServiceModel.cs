using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public class AdditionalServiceModel : OrderingBaseModel
    {
        public AdditionalServiceModel(string odsCode, EntityFramework.Models.Ordering.Order order, List<OrderItem> orderItems)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{order.CallOffId}";
            BackLinkText = "Go back";
            Title = $"Additional Services for {order.CallOffId}";
            OdsCode = odsCode;
            OrderDescription = order.Description;
            CallOffId = order.CallOffId.ToString();
            OrderItems = orderItems;
        }

        public string OrderDescription { get; set; }

        public string CallOffId { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}
