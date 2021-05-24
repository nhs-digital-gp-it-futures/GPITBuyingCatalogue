namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public class SupplierSearchSelectModel : OrderingBaseModel
    {
        public SupplierSearchSelectModel()
        {
            BackLinkText = "Go back";
            BackLink = "/order/organisation/03F/order/C010004-01/supplier/search"; // TOOD
            Title = "Suppliers found"; // TODO
        }
    }
}
