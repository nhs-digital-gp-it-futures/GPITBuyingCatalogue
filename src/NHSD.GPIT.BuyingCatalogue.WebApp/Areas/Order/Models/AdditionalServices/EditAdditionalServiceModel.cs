namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public class EditAdditionalServiceModel : OrderingBaseModel
    {
        public EditAdditionalServiceModel()
        {
            BackLink = "/order/organisation/03F/order/C010001-01"; // TODO
            BackLinkText = "Go back";
            Title = "Automated Arrivals information for C010001-01"; // TODO
        }
    }
}
