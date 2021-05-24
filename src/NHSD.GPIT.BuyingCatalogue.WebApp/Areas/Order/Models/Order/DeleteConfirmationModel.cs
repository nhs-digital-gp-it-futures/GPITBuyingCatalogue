namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class DeleteConfirmationModel : OrderingBaseModel
    {
        public DeleteConfirmationModel()
        {
            BackLinkText = "Go back to all orders";
            BackLink = "/order/organisation/03F"; // TOOD
            Title = "Order C010005-01 deleted"; // TODO
        }
    }
}
