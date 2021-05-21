namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Dashboard
{
    public class NewOrderDescriptionModel : OrderingBaseModel
    {
        public NewOrderDescriptionModel()
        {
            BackLinkText = "Go back";
            BackLink = "/order/organisation/03F/order/neworder"; // TODO
            Title = "Order description";
        }
    }
}
