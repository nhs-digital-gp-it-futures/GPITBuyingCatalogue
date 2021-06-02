namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public class SupplierSearchModel : OrderingBaseModel
    {
        public SupplierSearchModel(string odsCode, EntityFramework.Models.Ordering.Order order)
        {
            BackLinkText = "Go back";
            BackLink = $"/order/organisation/{odsCode}/order/{order.CallOffId}";
            Title = $"Find supplier information for {order.CallOffId}";
            OdsCode = odsCode;
        }

        public SupplierSearchModel()
        {
        }

        public string SearchString { get; set; }
    }
}
