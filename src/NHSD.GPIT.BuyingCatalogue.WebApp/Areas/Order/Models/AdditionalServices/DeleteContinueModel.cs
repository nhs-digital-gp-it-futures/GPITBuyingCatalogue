namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public class DeleteContinueModel : OrderingBaseModel
    {
        public DeleteContinueModel(string odsCode, string callOffId, string solutionName)
        {
            Title = $"{solutionName} deleted from {callOffId}";
            ContinueLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services";
        }

        public string ContinueLink { get; set; }
    }
}
