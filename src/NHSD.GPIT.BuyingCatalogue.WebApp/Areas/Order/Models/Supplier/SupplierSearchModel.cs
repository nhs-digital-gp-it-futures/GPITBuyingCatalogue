namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public class SupplierSearchModel : OrderingBaseModel
    {
        public SupplierSearchModel()
        {
            BackLinkText = "Go back";
            BackLink = "/order/organisation/03F/order/C010004-01"; // TOOD
            Title = "Find supplier information for C010004-01"; // TODO
        }
    }
}
