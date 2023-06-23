using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ContractBilling
{
    public class DeleteContractBillingItemModel : NavBaseModel
    {
        public DeleteContractBillingItemModel()
        {
        }

        public DeleteContractBillingItemModel(CallOffId callOffId, string internalOrgId, ContractBillingItem item)
        {
            CallOffId = callOffId;
            InternalOrgId = internalOrgId;
            MilestoneId = item.Id;
            MilestoneName = item.Milestone.Title;
        }

        public int MilestoneId { get; set; }

        public string MilestoneName { get; set; }

        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }
    }
}
