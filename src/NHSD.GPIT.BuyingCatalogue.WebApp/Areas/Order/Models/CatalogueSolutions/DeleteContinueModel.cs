namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public class DeleteContinueModel : OrderingBaseModel
    {
        public DeleteContinueModel(string odsCode, string callOffId, string solutionName)
        {
            Title = $"{solutionName} deleted from {callOffId}";
            ContinueLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions";
        }

        public string ContinueLink { get; set; }
    }
}
