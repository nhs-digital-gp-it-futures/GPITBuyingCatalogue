using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteCatalogueSolution
{
    public sealed class DeleteSolutionModel : OrderingBaseModel
    {
        public DeleteSolutionModel()
        {
        }

        public DeleteSolutionModel(string odsCode, CallOffId callOffId, CatalogueItemId solutionId, string solutionName, string orderDescription)
        {
            BackLink = $"/order/organisation/{odsCode}/order/{callOffId}/catalogue-solutions/{solutionId}";
            Title = $"Delete {solutionName} from {callOffId}?";
            OdsCode = odsCode;
            CallOffId = callOffId;
            SolutionId = solutionId;
            SolutionName = solutionName;
            OrderDescription = orderDescription;
        }

        public CallOffId CallOffId { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public string SolutionName { get; set; }

        public string OrderDescription { get; set; }
    }
}
