namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class DeleteContinueModel : OrderingBaseModel
    {
        public DeleteContinueModel()
        {
            Title = "Training Day at Practice deleted from C010000-01"; // TODO
            ContinueLink = "/order/organisation/03F/order/C010000-01"; // TODO - and possibly add to base class if more crop up
        }

        public string ContinueLink { get; set; }
    }
}
