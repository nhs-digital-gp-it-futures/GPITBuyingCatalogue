namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public class SupplierModel : OrderingBaseModel
    {
        public SupplierModel()
        {
            BackLinkText = "Go back";
            BackLink = "/order/organisation/03F/order/C010004-01"; // TOOD
            Title = "Supplier information for C010004-01"; // TODO
        }
    }
}
