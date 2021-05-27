namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class DeleteOrderModel : OrderingBaseModel
    {
        public DeleteOrderModel()
        {
        }

        public DeleteOrderModel(string odsCode, EntityFramework.Models.Ordering.Order order)
        {
            BackLinkText = "Go back";
            BackLink = $"/order/organisation/{odsCode}/order/{order.CallOffId}";
            Title = $"Delete order {order.CallOffId}?";
            Description = order.Description;
        }

        public string Description { get; set; }
    }
}
