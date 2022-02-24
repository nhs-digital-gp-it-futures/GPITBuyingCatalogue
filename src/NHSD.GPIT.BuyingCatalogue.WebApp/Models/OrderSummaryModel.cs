namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public sealed class OrderSummaryModel
    {
        public OrderSummaryModel()
        {
        }

        public OrderSummaryModel(EntityFramework.Ordering.Models.Order order)
        {
            Order = order;
        }

        public EntityFramework.Ordering.Models.Order Order { get; set; }

        public string AdviceText { get; set; }

        public string Title { get; set; }
    }
}
