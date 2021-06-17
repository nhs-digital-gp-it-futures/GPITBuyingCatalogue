namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class SummaryModel : OrderingBaseModel
    {
        public SummaryModel(string odsCode, EntityFramework.Models.Ordering.Order order)
        {
            BackLinkText = "Go back";
            BackLink = $"/order/organisation/{odsCode}";
            Title = $"Order summary for {order.CallOffId}";
            OdsCode = odsCode;
            Order = order;
        }

        public EntityFramework.Models.Ordering.Order Order { get; set; }
    }
}
