using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public sealed class OrderSummaryModel
    {
        public OrderSummaryModel()
        {
        }

        public OrderSummaryModel(Order order, ImplementationPlan defaultImplementationPlan)
        {
            Order = order;
            DefaultImplementationPlan = defaultImplementationPlan;
        }

        public Order Order { get; set; }

        public ImplementationPlan DefaultImplementationPlan { get; set; }

        public string DefaultBillingPaymentTrigger => DefaultImplementationPlan?.Milestones?.LastOrDefault()?.Title ?? "Bill on invoice";

        public bool HasSpecificRequirements => Order?.ContractFlags?.HasSpecificRequirements == true;

        public bool UseDefaultBilling => Order?.ContractFlags?.UseDefaultBilling == true;

        public bool UseDefaultDataProcessing => Order?.ContractFlags?.UseDefaultDataProcessing == true;

        public bool UseDefaultImplementationPlan => Order?.ContractFlags?.UseDefaultImplementationPlan == true;
    }
}
