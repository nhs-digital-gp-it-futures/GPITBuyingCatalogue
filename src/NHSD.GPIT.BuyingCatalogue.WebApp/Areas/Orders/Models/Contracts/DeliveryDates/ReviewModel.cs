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
        private readonly Dictionary<int, string> orderItemNames = new();

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

            orderItemNames = order.OrderItems.ToDictionary(
                x => x.Id,
                x => x.CatalogueItem.Name);

            OrderWrapper = orderWrapper;

            SolutionId = order.GetSolutionOrderItem().Id;
            AdditionalServiceIds = order.GetAdditionalServices().Select(x => x.Id).ToList();
            AssociatedServiceIds = order.GetAllAssociatedServices().Select(x => x.Id).ToList();
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public OrderType OrderType { get; set; }

        public string SolutionName { get; set; }

        public string PracticeReorganisationName { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public int? SolutionId { get; set; }

        public List<int> AdditionalServiceIds { get; set; } = new();

        public List<int> AssociatedServiceIds { get; set; } = new();

        public OrderWrapper OrderWrapper { get; set; } = new();

        public string OrderItemName(int orderItemId) => orderItemNames.ContainsKey(orderItemId)
            ? orderItemNames[orderItemId]
            : string.Empty;

        public List<DateTime?> OrderItemDates(int orderItemId)
        {
            return GetRecipientsForItem(orderItemId)
                .Select(x => x.DeliveryDate)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        public List<(string OdsCode, string Name)> OrderItemRecipients(int orderItemId, DateTime? deliveryDate)
        {
            return GetRecipientsForItem(orderItemId)
                .Where(x => x.DeliveryDate == deliveryDate)
                .OrderBy(x => x.RecipientName)
                .Select(x => (x.OdsCode, x.RecipientName))
                .ToList();
        }

        private List<OrderItemRecipientModel> GetRecipientsForItem(int orderItemId)
        {
            var orderItem = OrderWrapper.OrderItems.FirstOrDefault(x => x.Id == orderItemId);
            if (orderItem == null) return [];

            var item = orderItem.Parent.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService
                ? orderItem.Parent
                : orderItem;
            return OrderWrapper.DetermineOrderRecipients(item.CatalogueItemId)
                .Select(x => new OrderItemRecipientModel(x, orderItem.CatalogueItemId))
                .ToList();
        }
    }
}
