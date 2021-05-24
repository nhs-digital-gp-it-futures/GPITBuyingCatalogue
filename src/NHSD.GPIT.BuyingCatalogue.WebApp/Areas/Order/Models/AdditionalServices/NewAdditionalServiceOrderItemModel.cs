namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public class NewAdditionalServiceOrderItemModel : OrderingBaseModel
    {
        public NewAdditionalServiceOrderItemModel()
        {
            BackLink = "/order/organisation/03F/order/C010001-01/additional-services"; // TODO
            BackLinkText = "Go back";
            Title = "Document Management information for C010001-01"; // TODO
        }
    }
}
