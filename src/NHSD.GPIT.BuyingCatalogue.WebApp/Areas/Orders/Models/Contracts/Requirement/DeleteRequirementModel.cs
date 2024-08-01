using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.Requirement
{
    public class DeleteRequirementModel : NavBaseModel
    {
        public DeleteRequirementModel()
        {
        }

        public DeleteRequirementModel(CallOffId callOffId, string internalOrgId, EntityFramework.Ordering.Models.Requirement requirement)
        {
            CallOffId = callOffId;
            InternalOrgId = internalOrgId;
            ItemId = requirement.Id;
            AssociatedServiceName = requirement.OrderItem?.CatalogueItem?.Name;
            Requirement = requirement.Details;
        }

        public int ItemId { get; set; }

        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }

        public string AssociatedServiceName { get; set; }

        public string Requirement { get; set; }
    }
}
