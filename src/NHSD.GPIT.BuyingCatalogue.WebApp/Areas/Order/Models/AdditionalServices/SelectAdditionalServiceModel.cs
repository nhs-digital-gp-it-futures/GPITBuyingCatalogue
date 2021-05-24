namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public class SelectAdditionalServiceModel : OrderingBaseModel
    {
        public SelectAdditionalServiceModel()
        {
            BackLink = "/order/organisation/03F/order/C010001-01/additional-services"; // TODO
            BackLinkText = "Go back";
            Title = "Add Additional Service for C010001-01"; // TODO
        }
    }
}
