using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteOrder
{
    public sealed class DeleteOrderModel : OrderingBaseModel
    {
        public DeleteOrderModel()
        {
        }

        public DeleteOrderModel(string odsCode, EntityFramework.Ordering.Models.Order order)
        {
            BackLinkText = "Go back";
            BackLink = $"/order/organisation/{odsCode}/order/{order.CallOffId}";
            Title = $"Delete order {order.CallOffId}?";
            Description = order.Description;
            CallOffId = order.CallOffId;
            OdsCode = odsCode;
        }

        public string Description { get; set; }

        public CallOffId CallOffId { get; set; }
    }
}
