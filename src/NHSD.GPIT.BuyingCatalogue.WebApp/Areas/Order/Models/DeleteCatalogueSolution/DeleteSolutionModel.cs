using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteCatalogueSolution
{
    public sealed class DeleteSolutionModel : OrderingBaseModel
    {
        public DeleteSolutionModel()
        {
        }

        public DeleteSolutionModel(string internalOrgId, CallOffId callOffId, CatalogueItemId solutionId, string solutionName, string orderDescription)
        {
            Title = $"Delete {solutionName} from {callOffId}?";
            InternalOrgId = internalOrgId;
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
