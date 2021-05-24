namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public class SelectAdditionalServiceRecipientsModel : OrderingBaseModel
    {
        public SelectAdditionalServiceRecipientsModel()
        {
            BackLink = "/order/organisation/03F/order/C010001-01/additional-services"; // TODO
            BackLinkText = "Go back";
            Title = "Service Recipients for Document Management for C010001-01"; // TODO
        }
    }
}
