using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates
{
    public class ReviewModel : NavBaseModel
    {
        private readonly Dictionary<int, string> orderItemNames = [];

        public ReviewModel()
        {
        }

        public ReviewModel(OrderWrapper orderWrapper)
        {
            var order = orderWrapper.Order;
            InternalOrgId = order.OrderingParty.InternalIdentifier;
            CallOffId = order.CallOffId;
            OrderType = order.OrderType;
            SolutionName = order.OrderType.GetSolutionNameFromOrder(order);
            PracticeReorganisationName = order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient?.Name ?? string.Empty;
            DeliveryDate = order.DeliveryDate;

            var orderItems = order.GetOrderItemIds();
            if (orderItems.Count == 0)
            {
                throw new InvalidOperationException($"No order items found for CallOffId {CallOffId}");
            }

            OrderItemId = orderItems[0];

            orderItemNames = order.OrderItems.ToDictionary(
                x => x.Id,
                x => x.CatalogueItem.Name);

            var orderItem = orderWrapper.Order.OrderItem(OrderItemId);
            var item = orderItem.Parent?.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService
                ? orderItem.Parent
                : orderItem;

            AssociatedServiceIds = [.. order.GetAssociatedServices().Select(x => x.Id)];
            Recipients = orderWrapper.DetermineOrderRecipients(item).GroupBy(y => y.ParentSublocation.SublocationOrganisation.Name)
                .Select(x => new KeyValuePair<string, OrderSublocationRecipient[]>(
                    x.Key,
                    [.. x.OrderBy(y => y.RecipientOdsOrganisation.Name)]))
                .ToDictionary();
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public OrderType OrderType { get; set; }

        public string SolutionName { get; set; }

        public string PracticeReorganisationName { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public List<int> AssociatedServiceIds { get; set; } = [];

        public Dictionary<string, OrderSublocationRecipient[]> Recipients { get; set; }

        public int OrderItemId { get; set; }

        public string OrderItemName(int orderItemId) => orderItemNames.ContainsKey(orderItemId)
            ? orderItemNames[orderItemId]
            : string.Empty;
    }
}
