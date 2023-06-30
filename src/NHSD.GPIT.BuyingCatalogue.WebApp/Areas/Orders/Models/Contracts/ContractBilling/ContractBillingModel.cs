using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.ContractBilling
{
    public class ContractBillingModel : NavBaseModel
    {
        public ContractBillingModel()
        {
        }

        public ContractBillingModel(EntityFramework.Ordering.Models.ContractBilling bespokeBilling)
        {
            BespokeBilling = bespokeBilling;
        }

        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }

        public EntityFramework.Ordering.Models.ContractBilling BespokeBilling { get; set; }

        public bool HasBespokeBilling => BespokeBilling != null && BespokeBilling.ContractBillingItems.Any();

        public string BespokeBillingLabelText => "Bespoke milestones and payment triggers";
    }
}
