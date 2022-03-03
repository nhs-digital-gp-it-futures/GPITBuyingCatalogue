using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.DeleteAssociatedService
{
    public sealed class DeleteAssociatedServiceModel : OrderingBaseModel
    {
        public DeleteAssociatedServiceModel()
        {
        }

        public DeleteAssociatedServiceModel(string internalOrgId, CallOffId callOffId, CatalogueItemId catalogueItemId, string solutionName, string orderDescription)
        {
            Title = $"Delete {solutionName} from {callOffId}?";
            InternalOrgId = internalOrgId;
            CallOffId = callOffId;
            CatalogueItemId = catalogueItemId;
            SolutionName = solutionName;
            OrderDescription = orderDescription;
        }

        public CallOffId CallOffId { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public string SolutionName { get; set; }

        public string OrderDescription { get; set; }
    }
}
