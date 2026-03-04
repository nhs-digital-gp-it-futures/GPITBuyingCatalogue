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

        public bool HasBespokeMilestones => BespokePlan != null && BespokePlan.Milestones.Count != 0;

        public bool HasBespokeBilling => BespokeBilling != null && BespokeBilling.ContractBillingItems.Count != 0;

        public bool HasSpecificRequirements => BespokeBilling != null && BespokeBilling.Requirements.Count != 0;

        public AmendOrderItemModel BuildAmendOrderItemModel(OrderItem solution)
        {
            var model = new AmendOrderItemModel(
                CallOffId,
                Order.OrderType,
                RolledUp.GetOrderRecipients().ToList(),
                Previous?.FlattenedRecipients.ToList(),
                solution,
                Previous?.OrderItem(solution.CatalogueItemId),
                new FundingTypeDescriptionModel(OrderWrapper.FundingTypesForItem(solution.CatalogueItemId)))
            {
                OrderWrapper = OrderWrapper,
            };

            if (Order.OrderType.MergerOrSplit)
            {
                model.PracticeReorganisationName =
                    Order.AssociatedServicesOnlyDetails.PracticeReorganisationNameAndCode;
            }

            return model;
        }
    }
}
