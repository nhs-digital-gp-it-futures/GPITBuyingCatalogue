using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Calculations;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Orders
{
    public sealed class SummaryModel : OrderingBaseModel
    {
        public SummaryModel(OrderWrapper orderWrapper, string internalOrgId, bool hasSubsequentRevisions, ImplementationPlan defaultPlan)
        {
            InternalOrgId = internalOrgId;
            OrderWrapper = orderWrapper;
            DefaultPlan = defaultPlan;
            HasSubsequentRevisions = hasSubsequentRevisions;
        }

        public OrderWrapper OrderWrapper { get; }

        public CallOffId CallOffId => OrderWrapper.Order.CallOffId;

        public Order Order => OrderWrapper.Order;

        public Order Previous => OrderWrapper.Previous;

        public Order RolledUp => OrderWrapper.RolledUp;

        public string AdviceText { get; set; }

        public bool CanBeTerminated => Order.OrderStatus == OrderStatus.Completed && !HasSubsequentRevisions;

        public bool CanBeAmended => !Order.OrderType.AssociatedServicesOnly && CanBeTerminated && !Order.ContractExpired;

        public bool HasSubsequentRevisions { get; init; }

        public ImplementationPlan DefaultPlan { get; set; }

        public ImplementationPlan BespokePlan => Order.Contract?.ImplementationPlan;

        public ContractBilling BespokeBilling => Order.Contract?.ContractBilling;

        public bool HasBespokeMilestones => BespokePlan != null && BespokePlan.Milestones.Any();

        public string DefaultMilestoneLabelText => "Default milestones and payment triggers";

        public string BespokeMilestoneLabelText => "Bespoke milestones and payment triggers";

        public bool HasBespokeBilling => BespokeBilling != null && BespokeBilling.ContractBillingItems.Any();

        public bool HasSpecificRequirements => BespokeBilling != null && BespokeBilling.Requirements.Any();

        public string BespokeBillingLabelText => "Bespoke milestones and payment triggers";

        public string RequirementLabelText => "Specific requirements";

        public string ButtonLabelText => Order.OrderStatus == OrderStatus.InProgress && OrderWrapper.CanComplete()
            ? "Complete order"
            : Order.OrderStatus == OrderStatus.Terminated
                ? "Download summary"
                : "Order summary";

        public string ButtonAdviceText => Order.OrderStatus == OrderStatus.InProgress && OrderWrapper.CanComplete()
            ?
            "Make sure you are happy with the order before marking it as complete."
            : Order.OrderStatus == OrderStatus.Terminated
                ? "You can download a summary of this terminated contract for your records."
                : "You can download and review your order summary here.";

        public AmendOrderItemModel BuildAmendOrderItemModel(OrderItem item)
        {
            return new AmendOrderItemModel(
                CallOffId,
                Order.OrderType,
                RolledUp.OrderRecipients,
                Previous?.OrderRecipients,
                item,
                Previous?.OrderItem(item.CatalogueItemId),
                Order.IsAmendment,
                new FundingTypeDescriptionModel(OrderWrapper.FundingTypesForItem(item.CatalogueItemId)))
            { InternalOrgId = InternalOrgId, CanEdit = Order.OrderStatus == OrderStatus.InProgress };
        }

        public OrderTotalModel BuildOrderTotals()
        {
            return new OrderTotalModel(
                 Order.OrderType,
                 Order.MaximumTerm,
                 Order.TotalOneOffCost(null),
                 Order.TotalMonthlyCost(null),
                 Order.TotalAnnualCost(null),
                 OrderWrapper.TotalCost());
        }
    }
}
