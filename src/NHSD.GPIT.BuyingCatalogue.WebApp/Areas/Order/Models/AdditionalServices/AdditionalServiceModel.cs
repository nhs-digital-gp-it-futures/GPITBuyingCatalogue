namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public class AdditionalServiceModel : OrderingBaseModel
    {
        public AdditionalServiceModel()
        {
            BackLink = "/order/organisation/03F/order/C010001-01"; // TODO
            BackLinkText = "Go back";
            Title = "Additional Services for C010005"; // TODO
        }
    }
}
