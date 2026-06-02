using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Routing;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Orders.Models.Contracts.DeliveryDates
{
    public class EditDatesModel : NavBaseModel
    {
        public EditDatesModel()
        {
        }

        public EditDatesModel(OrderWrapper orderWrapper, int orderItemId, RoutingSource? source = null)
        {
            var order = orderWrapper.Order;
            InternalOrgId = order.OrderingParty.InternalIdentifier;
            CallOffId = order.CallOffId;
            OrderType = order.OrderType;
            SolutionName = order.OrderType.GetSolutionNameFromOrder(order);
            PracticeReorganisationName = order.AssociatedServicesOnlyDetails.PracticeReorganisationRecipient?.Name ?? string.Empty;
            OrderItemId = orderItemId;
            DeliveryDate = order.DeliveryDate;
            Source = source;
            DisplayEditLink = order.GetPreviousOrderItemId(orderItemId) == null;

            var orderItem = order.OrderItem(orderItemId);
            var item = IsParentAdditionalService(orderItem)
                ? orderItem.Parent
                : orderItem;

            CatalogueItemType = orderItem.CatalogueItem.CatalogueItemType;
            Description = orderItem.Parent?.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService
                    ? $"{orderItem.Parent.CatalogueItem.Name} - {orderItem.CatalogueItem.Name}"
                    : orderItem.CatalogueItem.Name;
            CatalogueItemTypeSuffix = IsParentAdditionalService(orderItem)
                    ? "Associated service for an Additional service"
                    : CatalogueItemType.Name();

            ICollection<OrderSublocationRecipient> recipients = orderWrapper.DetermineOrderRecipients(item.CatalogueItemId)
                .Where(x => !string.Equals(
                    x.RecipientOdsCode,
                    order.AssociatedServicesOnlyDetails.PracticeReorganisationOdsCode))
                .ToList();

            RecipientDateModel[] recipientDates = recipients
                .Select(x => new RecipientDateModel(
                    x,
                    x.GetDeliveryDateForItem(orderItem.Id) ?? DeliveryDate,
                    order.CommencementDate!.Value))
                .OrderBy(y => y.Description)
                .ToArray();

            Recipients = OrderType.MergerOrSplit
                ?
                [
                    new KeyValuePair<string, RecipientDateModel[]>(
                        OrderType.Value == OrderTypeEnum.AssociatedServiceSplit
                            ? "Service recipients receiving patients"
                            : "Service recipients to be merged",
                        recipientDates),
                ]
                : recipientDates
                    .GroupBy(x => x.Location)
                    .Select(x => new KeyValuePair<string, RecipientDateModel[]>(
                        x.Key,
                        x.OrderBy(y => y.Description).ToArray()))
                    .ToList();
        }

        public string InternalOrgId { get; set; }

        public CallOffId CallOffId { get; set; }

        public OrderType OrderType { get; set; }

        public string SolutionName { get; set; }

        public string PracticeReorganisationName { get; set; }

        public CatalogueItemId CatalogueItemId { get; set; }

        public int OrderItemId { get; set; }

        public CatalogueItemType CatalogueItemType { get; set; }

        public string CatalogueItemTypeSuffix { get; set; }

        public RoutingSource? Source { get; set; }

        public string Description { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public bool DisplayEditLink { get; set; }

        public List<KeyValuePair<string, RecipientDateModel[]>> Recipients { get; set; }

        private static bool IsParentAdditionalService(OrderItem orderItem)
        {
            return orderItem.Parent?.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService;
        }
    }
}
