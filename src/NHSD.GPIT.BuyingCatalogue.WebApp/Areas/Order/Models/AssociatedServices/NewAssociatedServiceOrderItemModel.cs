namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class NewAssociatedServiceOrderItemModel : OrderingBaseModel
    {
        public NewAssociatedServiceOrderItemModel()
        {
            BackLink = "/order/organisation/03F/order/C010000-01/associated-services"; // TODO
            BackLinkText = "Go back";
            Title = "Training Day at Practice information for C010000-01"; // TODO
        }
    }
}
