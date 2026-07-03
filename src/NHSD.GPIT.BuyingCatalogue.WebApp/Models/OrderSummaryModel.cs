using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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

        public IEnumerable<OrderItem> AssociatedServicesForCurrentOrder => OrderWrapper.Order.GetAssociatedServices();

        public IEnumerable<IGrouping<CallOffId, OrderItem>> PreviousAssociatedServicesGrouping =>
            OrderWrapper.PreviousOrders.SelectMany(order => order.GetAssociatedServices())
                .GroupBy(associatedService => associatedService.Order.CallOffId);

        public ImplementationPlan DefaultImplementationPlan { get; set; }

        public ImplementationPlan BespokePlan => Order.Contract?.ImplementationPlan;

        public ContractBilling BespokeBilling => Order.Contract?.ContractBilling;

        public bool UseDefaultDataProcessing => Order?.ContractFlags?.UseDefaultDataProcessing == true;

        public bool HasBespokeMilestones => BespokePlan != null && BespokePlan.Milestones.Any();

        public bool HasBespokeBilling => BespokeBilling != null && BespokeBilling.ContractBillingItems.Any();

        public bool HasSpecificRequirements => BespokeBilling != null && BespokeBilling.Requirements.Any();

        public IDictionary<int?, HashSet<OrderItem>> AssociatedServicesForAdditionalServices =>
            Order.GetAllAssociatedServices()
                .Where(service => service.Parent.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService)
                .GroupBy(service => service.ParentId)
                .ToDictionary(group => group.Key, group => group.ToHashSet());

        public Dictionary<CatalogueItemId, Dictionary<CallOffId, List<OrderItem>>> PreviousAssociatedServicesForAdditionalServices =>
            OrderWrapper.PreviousOrders.SelectMany(order => order.GetAllAssociatedServices())
                .Where(associatedService => associatedService.Parent.CatalogueItem.CatalogueItemType
                    == CatalogueItemType.AdditionalService)
                .GroupBy(associatedService => associatedService.Parent.CatalogueItemId)
                .ToDictionary(
                    grouping => grouping.Key,
                    grouping => grouping.GroupBy(associatedService => associatedService.Order.CallOffId)
                        .ToDictionary(innerGroup => innerGroup.Key, innerGroup => innerGroup.ToList()));

        public AmendOrderItemModel BuildAmendOrderItemModel(OrderItem solution, string solutionName = null, bool fromPreviousRevision = false)
        {
            var orderLinkedList = new LinkedList<Order>([.. OrderWrapper.PreviousOrders, OrderWrapper.Order]);
            var previous = orderLinkedList.Find(solution.Order)?.Previous;
            var recipients = solution.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService
                ? solution.Order.DetermineOrderRecipients(previous?.Value, solution.Id)
                : RolledUp.GetOrderRecipients().ToList();
            var previousRecipients = solution.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService
                ? []
                : Previous?.GetOrderRecipients().ToList();
            var itemName = solution.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService && solutionName != null
                ? $"{solutionName} - {solution.CatalogueItem.Name}"
                : solution.CatalogueItem.Name;
            var callOffId = solution.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService
                ? solution.Order.CallOffId
                : CallOffId;
            var model = new AmendOrderItemModel(
                callOffId,
                Order.OrderType,
                recipients,
                previousRecipients,
                solution,
                OrderWrapper.PreviousOrders.AsEnumerable().LastOrDefault()?.OrderItem(solution.CatalogueItemId),
                new FundingTypeDescriptionModel(OrderWrapper.FundingTypesForItem(solution.CatalogueItemId)))
            {
                OrderWrapper = OrderWrapper,
                ItemName = itemName,
                FromPreviousRevision = fromPreviousRevision,
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
