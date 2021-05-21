namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class EditAssociatedServiceModel : OrderingBaseModel
    {
        public EditAssociatedServiceModel()
        {
            BackLink = "/order/organisation/03F/order/C010001-01/associated-services"; // TODO
            BackLinkText = "Go back";
            Title = "Anywhere Consult – Integrated Device information for C010001-01"; // TODO
        }
    }
}
