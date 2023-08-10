using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders
{
    public sealed class SummaryModel : OrderingBaseModel
    {
        public SummaryModel(OrderWrapper orderWrapper, string internalOrgId, bool hasSubsequentRevisions, ImplementationPlan defaultPlan = null)
        {
            InternalOrgId = internalOrgId;
            OrderWrapper = orderWrapper;
            DefaultPlan = defaultPlan;
            CanBeTerminated = Order.OrderStatus == OrderStatus.Completed && !hasSubsequentRevisions;
            CanBeAmended = !Order.AssociatedServicesOnly && CanBeTerminated;
        }

        public OrderWrapper OrderWrapper { get; }

        public CallOffId CallOffId => OrderWrapper.Order.CallOffId;

        public Order Order => OrderWrapper.Order;

        public Order Previous => OrderWrapper.Previous;

        public Order RolledUp => OrderWrapper.RolledUp;

        public string AdviceText { get; set; }

        public bool CanBeTerminated { get; init; }

        public bool CanBeAmended { get; init; }

        public ImplementationPlan DefaultPlan { get; set; }

        public bool UseDefaultDataProcessing => Order?.ContractFlags?.UseDefaultDataProcessing == true;

        public bool UseDefaultImplementationPlan => Order?.ContractFlags?.UseDefaultImplementationPlan == true;

        public string BillingPaymentTrigger => DefaultPlan?.Milestones?.LastOrDefault()?.Title ?? "Bill on invoice";

        public bool HasBespokeBilling => Order?.ContractFlags?.UseDefaultBilling == false;

        public bool HasSpecificRequirements => Order?.ContractFlags?.HasSpecificRequirements == true;

        public FundingTypeDescriptionModel FundingTypeDescription(CatalogueItemId catalogueItemId)
        {
            return new FundingTypeDescriptionModel(OrderWrapper.FundingTypesForItem(catalogueItemId));
        }
    }
}
