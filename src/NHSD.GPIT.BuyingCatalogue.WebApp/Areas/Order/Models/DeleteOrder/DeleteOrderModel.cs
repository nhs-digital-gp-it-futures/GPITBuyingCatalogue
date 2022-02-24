using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteOrder
{
    public sealed class DeleteOrderModel : OrderingBaseModel
    {
        public DeleteOrderModel()
        {
        }

        public DeleteOrderModel(string internalOrgId, EntityFramework.Ordering.Models.Order order)
        {
            Title = $"Delete order {order.CallOffId}?";
            Description = order.Description;
            CallOffId = order.CallOffId;
            InternalOrgId = internalOrgId;
        }

        public string Description { get; set; }

        public CallOffId CallOffId { get; set; }
    }
}
