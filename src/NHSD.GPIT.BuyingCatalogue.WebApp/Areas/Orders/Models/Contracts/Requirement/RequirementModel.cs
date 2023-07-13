using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.Requirement
{
    public class RequirementModel : NavBaseModel
    {
        public RequirementModel()
        {
        }

        public RequirementModel(EntityFramework.Ordering.Models.ContractBilling contractBilling)
        {
            ContractBilling = contractBilling;
        }

        public CallOffId CallOffId { get; set; }

        public string InternalOrgId { get; set; }

        public EntityFramework.Ordering.Models.ContractBilling ContractBilling { get; set; }

        public bool HasRequirements => ContractBilling != null && ContractBilling.Requirements.Any();
    }
}
