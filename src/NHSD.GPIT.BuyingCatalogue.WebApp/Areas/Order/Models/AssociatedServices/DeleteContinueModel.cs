namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AssociatedServices
{
    public class DeleteContinueModel : OrderingBaseModel
    {
        public DeleteContinueModel(string odsCode, string callOffId, string solutionName)
        {
            Title = $"{solutionName} deleted from {callOffId}";
            ContinueLink = $"/order/organisation/{odsCode}/order/{callOffId}/associated-services";
        }

        public string ContinueLink { get; set; }
    }
}
