namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public sealed class SummaryModel : OrderingBaseModel
    {
        public SummaryModel()
        {
        }

        public SummaryModel(string odsCode, EntityFramework.Ordering.Models.Order order)
        {
            OdsCode = odsCode;
            Order = order;
        }

        public EntityFramework.Ordering.Models.Order Order { get; set; }

        public string AdviceText { get; set; }
    }
}
