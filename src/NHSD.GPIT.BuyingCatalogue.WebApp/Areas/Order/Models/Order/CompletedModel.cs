using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Order.Models.Order
{
    public class CompletedModel : NavBaseModel
    {
        public CompletedModel(string internalOrgId, EntityFramework.Ordering.Models.Order order)
        {
            InternalOrgId = internalOrgId;
            CallOffId = order.CallOffId;
            Order = order;
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public EntityFramework.Ordering.Models.Order Order { get; set; }

        public bool SupportingDocumentsRequired =>
            Order?.ContractFlags?.HasSpecificRequirements == true
            || Order?.ContractFlags?.UseDefaultBilling == false
            || Order?.ContractFlags?.UseDefaultDataProcessing == false
            || Order?.ContractFlags?.UseDefaultImplementationPlan == false;

        public bool HasBespokeBilling =>
            Order?.ContractFlags?.HasSpecificRequirements == true
            || Order?.ContractFlags?.UseDefaultBilling == false;

        public bool HasBespokeDataProcessing => Order?.ContractFlags?.UseDefaultDataProcessing == false;

        public bool HasBespokeImplementationPlan => Order?.ContractFlags?.UseDefaultImplementationPlan == false;
    }
}
