namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public sealed class SupplierSearchModel : OrderingBaseModel
    {
        public SupplierSearchModel(string odsCode, EntityFramework.Ordering.Models.Order order)
        {
            Title = $"Find supplier information for {order.CallOffId}";
            OdsCode = odsCode;
        }

        public SupplierSearchModel()
        {
        }

        public string SearchString { get; set; }
    }
}
