namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class AssociatedServiceModel : OrderingBaseModel
    {
        public AssociatedServiceModel()
        {
            BackLink = "/order/organisation/03F/order/C010000-01"; // TODO
            BackLinkText = "Go back";
            Title = "Associated Services for C010000-01"; // TODO
        }
    }
}
