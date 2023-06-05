using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders
{
    public class CompletedModel : NavBaseModel
    {
        public CompletedModel(string internalOrgId, Order order)
        {
            InternalOrgId = internalOrgId;
            CallOffId = order.CallOffId;
            Order = order;
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public Order Order { get; set; }

        public bool SupportingDocumentsRequired =>
            Order?.ContractFlags?.HasSpecificRequirements == true
            || Order?.ContractFlags?.UseDefaultBilling == false
            || Order?.ContractFlags?.UseDefaultDataProcessing == true
            || Order?.ContractFlags?.UseDefaultImplementationPlan == false;

        public bool HasBespokeBilling =>
            Order?.ContractFlags?.HasSpecificRequirements == true
            || Order?.ContractFlags?.UseDefaultBilling == false;

        public bool DataProcessingSeen => Order?.ContractFlags?.UseDefaultDataProcessing == false;

        public bool HasBespokeImplementationPlan => Order?.ContractFlags?.UseDefaultImplementationPlan == false;
    }
}
