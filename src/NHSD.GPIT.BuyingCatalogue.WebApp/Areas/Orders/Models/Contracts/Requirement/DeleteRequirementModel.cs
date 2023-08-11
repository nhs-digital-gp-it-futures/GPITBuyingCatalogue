using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.Requirement
{
    public class DeleteRequirementModel : NavBaseModel
    {
        public DeleteRequirementModel()
        {
        }

        public DeleteRequirementModel(CallOffId callOffId, string internalOrgId, int itemId)
        {
            CallOffId = callOffId;
            InternalOrgId = internalOrgId;
            ItemId = itemId;
        }

        public int ItemId { get; set; }

        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }
    }
}
