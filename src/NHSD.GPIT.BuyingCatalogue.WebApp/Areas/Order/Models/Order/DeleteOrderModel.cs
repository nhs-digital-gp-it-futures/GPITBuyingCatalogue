namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class DeleteOrderModel : OrderingBaseModel
    {
        public DeleteOrderModel()
        {
        }

        public DeleteOrderModel(string odsCode, string callOffId, EntityFramework.Models.Ordering.Order order)
        {
            BackLinkText = "Go back";
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}";
            Title = $"Delete order {callOffId}?";
            Description = order.Description;
        }

        public string Description { get; set; }
    }
}
