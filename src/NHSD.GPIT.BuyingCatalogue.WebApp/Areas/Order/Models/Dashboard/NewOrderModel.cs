namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard
{
    public class NewOrderModel : OrderingBaseModel
    {
        public NewOrderModel(string odsCode)
        {
            BackLinkText = "Go back to all orders";
            BackLink = $"/order/organisation/{odsCode}";
            Title = "New order";
            OdsCode = odsCode;
        }
    }
}
