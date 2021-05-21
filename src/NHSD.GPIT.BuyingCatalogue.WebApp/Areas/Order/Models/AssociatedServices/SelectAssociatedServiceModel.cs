namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class SelectAssociatedServiceModel : OrderingBaseModel
    {
        public SelectAssociatedServiceModel()
        {
            BackLink = "/order/organisation/03F/order/C010000-01/associated-services"; // TODO
            BackLinkText = "Go back";
            Title = "Add Associated Service for C010000-01"; // TODO
        }
    }
}
