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
            ItemId = item.Id;
            ItemName = item.Milestone.Title;
            ItemDescription = item.Milestone.PaymentTrigger;
        }

        public int ItemId { get; set; }

        public string ItemName { get; set; }

        public string ItemDescription { get; set; }

        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }
    }
}
