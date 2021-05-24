namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class DeleteOrderModel : OrderingBaseModel
    {
        public DeleteOrderModel()
        {
            BackLinkText = "Go back";
            BackLink = "/order/organisation/03F/order/C010005-01"; // TODO
            Title = "Delete order C010005-01?"; // TODO
        }
    }
}
