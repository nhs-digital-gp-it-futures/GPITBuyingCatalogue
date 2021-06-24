namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public sealed class SummaryModel : OrderingBaseModel
    {
        public SummaryModel(string odsCode, EntityFramework.Ordering.Models.Order order)
        {
            BackLinkText = "Go back";
            BackLink = $"/order/organisation/{odsCode}";
            Title = $"Order summary for {order.CallOffId}";
            OdsCode = odsCode;
            Order = order;
        }

        public EntityFramework.Ordering.Models.Order Order { get; set; }
    }
}
