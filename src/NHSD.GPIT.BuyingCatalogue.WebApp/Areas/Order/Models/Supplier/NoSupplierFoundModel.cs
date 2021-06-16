namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Supplier
{
    public class NoSupplierFoundModel : OrderingBaseModel
    {
        public NoSupplierFoundModel(string odsCode, string callOffId)
        {
            BackLinkText = "Go back to search";
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/supplier/search";
        }
    }
}
