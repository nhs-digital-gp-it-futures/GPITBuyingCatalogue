using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.CatalogueSolutions
{
    public sealed class DeleteContinueModel : OrderingBaseModel
    {
        public DeleteContinueModel(string odsCode, CallOffId callOffId, string solutionName)
        {
            Title = $"{solutionName} deleted from {callOffId}";
            ContinueLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions";
        }

        public string ContinueLink { get; set; }
    }
}
