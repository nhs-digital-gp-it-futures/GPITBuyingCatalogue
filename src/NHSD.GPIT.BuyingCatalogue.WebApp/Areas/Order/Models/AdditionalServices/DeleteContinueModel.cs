using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.AdditionalServices
{
    public sealed class DeleteContinueModel : OrderingBaseModel
    {
        public DeleteContinueModel(string odsCode, CallOffId callOffId, string solutionName)
        {
            Title = $"{solutionName} deleted from {callOffId}";
            ContinueLink = $"/order/organisation/{odsCode}/order/{callOffId}/additional-services";
        }

        public string ContinueLink { get; set; }
    }
}
