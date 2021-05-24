namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public class DeleteContinueModel : OrderingBaseModel
    {
        public DeleteContinueModel()
        {
            Title = "Anywhere Consult deleted from C010001-01"; // TODO
            ContinueLink = "/order/organisation/03F/order/C010001-01/catalogue-solutions"; // TODO - and possibly add to base class if more crop up
        }

        public string ContinueLink { get; set; }
    }
}
