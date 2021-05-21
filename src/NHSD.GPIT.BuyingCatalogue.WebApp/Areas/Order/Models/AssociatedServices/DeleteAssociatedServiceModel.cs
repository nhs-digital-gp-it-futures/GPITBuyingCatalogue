namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class DeleteAssociatedServiceModel : OrderingBaseModel
    {
        public DeleteAssociatedServiceModel()
        {
            BackLink = "/order/organisation/03F/order/C010000-01/associated-services/10000-S-003"; // TODO
            BackLinkText = "Go back";
            Title = "Delete Training Day at Practice from C010000-01?"; // TODO
        }
    }
}
