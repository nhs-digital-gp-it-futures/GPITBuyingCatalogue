using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    public sealed class OrderSummaryModel
    {
        public OrderSummaryModel()
        {
        }

        public OrderSummaryModel(OrderWrapper orderWrapper, ImplementationPlan defaultImplementationPlan)
        {
            OrderWrapper = orderWrapper;
            DefaultImplementationPlan = defaultImplementationPlan;
        }

        public OrderWrapper OrderWrapper { get; }

        public CallOffId CallOffId => OrderWrapper.Order.CallOffId;

        public Order Order => OrderWrapper.Order;

        public Order Previous => OrderWrapper.Previous;

        public Order RolledUp => OrderWrapper.RolledUp;

        public ImplementationPlan DefaultImplementationPlan { get; set; }

        public string DefaultBillingPaymentTrigger => DefaultImplementationPlan?.Milestones?.LastOrDefault()?.Title ?? "Bill on invoice";

        public bool HasSpecificRequirements => Order?.ContractFlags?.HasSpecificRequirements == true;

        public bool UseDefaultBilling => Order?.ContractFlags?.UseDefaultBilling == true;

        public bool UseDefaultDataProcessing => Order?.ContractFlags?.UseDefaultDataProcessing == true;

        public bool UseDefaultImplementationPlan => Order?.ContractFlags?.UseDefaultImplementationPlan == true;

        public FundingTypeDescriptionModel FundingTypeDescription(CatalogueItemId catalogueItemId)
        {
            return new FundingTypeDescriptionModel(OrderWrapper.FundingTypesForItem(catalogueItemId));
        }
    }
}
