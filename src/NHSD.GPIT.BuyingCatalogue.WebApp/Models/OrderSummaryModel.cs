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

        public ImplementationPlan BespokePlan => Order.Contract?.ImplementationPlan;

        public ContractBilling BespokeBilling => Order.Contract?.ContractBilling;

        public bool UseDefaultDataProcessing => Order?.ContractFlags?.UseDefaultDataProcessing == true;

        public bool HasBespokeMilestones => BespokePlan != null && BespokePlan.Milestones.Any();

        public bool HasBespokeBilling => BespokeBilling != null && BespokeBilling.ContractBillingItems.Any();

        public bool HasSpecificRequirements => BespokeBilling != null && BespokeBilling.Requirements.Any();

        public AmendOrderItemModel BuildAmendOrderItemModel(OrderItem solution)
        {
            var model = new AmendOrderItemModel(
                CallOffId,
                Order.OrderType,
                RolledUp.OrderRecipients,
                Previous?.OrderRecipients,
                solution,
                Previous?.OrderItem(solution.CatalogueItemId),
                Order.IsAmendment,
                new FundingTypeDescriptionModel(OrderWrapper.FundingTypesForItem(solution.CatalogueItemId)));

            if (Order.OrderType.MergerOrSplit)
            {
                model.PracticeReorganisationName = Order.AssociatedServicesOnlyDetails.PracticeReorganisationNameAndCode;
            }

            return model;
        }
    }
}
