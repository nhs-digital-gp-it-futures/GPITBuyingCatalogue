using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public sealed class CatalogueSolutionsModel : OrderingBaseModel
    {
        public CatalogueSolutionsModel(string odsCode, EntityFramework.Ordering.Models.Order order, List<OrderItem> orderItems)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{order.CallOffId}";
            BackLinkText = "Go back";
            Title = $"Catalogue Solution for {order.CallOffId}";
            OdsCode = odsCode;
            OrderDescription = order.Description;
            CallOffId = order.CallOffId.ToString();
            OrderItems = orderItems;
        }

        public string OrderDescription { get; set; }

        // TODO: should be of type CallOffId
        public string CallOffId { get; set; }

        public List<OrderItem> OrderItems { get; set; }
    }
}
