using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates
{
    public class ReviewModel : NavBaseModel
    {
        private readonly Dictionary<CatalogueItemId, string> orderItemNames = new();

        public ReviewModel()
        {
        }

        public ReviewModel(OrderWrapper orderWrapper)
        {
            var order = orderWrapper.Order;
            InternalOrgId = order.OrderingParty.InternalIdentifier;
            CallOffId = order.CallOffId;
            DeliveryDate = order.DeliveryDate;

            orderItemNames = order.OrderItems.ToDictionary(
                x => x.CatalogueItemId,
                x => x.CatalogueItem.Name);

            OrderWrapper = orderWrapper;

            SolutionId = order.GetSolution()?.CatalogueItemId;
            AdditionalServiceIds = order.GetAdditionalServices().Select(x => x.CatalogueItemId).ToList();
            AssociatedServiceIds = order.GetAssociatedServices().Select(x => x.CatalogueItemId).ToList();
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public CatalogueItemId? SolutionId { get; set; }

        public List<CatalogueItemId> AdditionalServiceIds { get; set; } = new();

        public List<CatalogueItemId> AssociatedServiceIds { get; set; } = new();

        public OrderWrapper OrderWrapper { get; set; } = new();

        public string OrderItemName(CatalogueItemId catalogueItemId) => orderItemNames.ContainsKey(catalogueItemId)
            ? orderItemNames[catalogueItemId]
            : string.Empty;

        public List<DateTime?> OrderItemDates(CatalogueItemId catalogueItemId)
        {
            return GetRecipientsForItem(catalogueItemId)
                .Select(x => x.DeliveryDate)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        public List<(string OdsCode, string Name)> OrderItemRecipients(CatalogueItemId catalogueItemId, DateTime? deliveryDate)
        {
            return GetRecipientsForItem(catalogueItemId)
                .Where(x => x.DeliveryDate == deliveryDate)
                .OrderBy(x => x.RecipientName)
                .Select(x => (x.OdsCode, x.RecipientName))
                .ToList();
        }

        private List<OrderItemRecipientModel> GetRecipientsForItem(CatalogueItemId catalogueItemId)
        {
            return OrderWrapper.DetermineOrderRecipients(catalogueItemId)
                .Select(x => new OrderItemRecipientModel(x, catalogueItemId))
                .ToList();
        }
    }
}
