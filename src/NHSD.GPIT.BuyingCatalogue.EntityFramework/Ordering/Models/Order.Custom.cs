using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public partial class Order : ICloneable<Order>
    {
        public const string LocalFunding = "Local";
        public const string CentralFunding = "Central";

        [JsonIgnore]
        public bool HasSingleFundingType =>
            SelectedFramework.HasSingleFundingType || OrderingParty.OrganisationType == OrganisationType.GP;

        [JsonIgnore]
        public EndDate EndDate => new(CommencementDate, MaximumTerm);

        [JsonIgnore]
        public bool ContractExpired => EndDate.ContractExpired;

        [JsonIgnore]
        public bool IsAmendment => CallOffId.IsAmendment;

        public void Complete()
        {
            Completed = DateTime.UtcNow;
        }

        public bool CanComplete(
            ICollection<OrderSublocationRecipient> orderRecipients,
            ICollection<OrderItem> orderItems)
        {
            return
                !string.IsNullOrWhiteSpace(Description)
                && OrderingPartyContact is not null
                && Supplier is not null
                && CommencementDate is not null
                && (HasValidCatalogueItems() || HasAssociatedService())
                && !HasSublocationsWithNoRecipients()
                && OrderItems.Count > 0
                && HaveAllDeliveryDates(orderRecipients)
                && HaveAllQuantities(orderRecipients)
                && orderItems.All(oi => oi.OrderItemFunding is not null)
                && ContractFlags is not null
                && (!OrderType.ImplementationPlanRequired || Contract?.ImplementationPlan is not null)
                && (IsAmendment || !HasAssociatedService() || Contract?.ContractBilling is not null)
                && ContractFlags?.UseDefaultDataProcessing == true
                && OrderStatus == OrderStatus.InProgress;
        }

        public bool HaveAllDeliveryDates(ICollection<OrderSublocationRecipient> orderRecipients)
        {
            return OrderItems.All(x => orderRecipients.AllDeliveryDatesEntered(x.CatalogueItemId));
        }

        public CatalogueItemId? GetSolutionId()
        {
            return OrderType.AssociatedServicesOnly
                ? AssociatedServicesOnlyDetails.SolutionId
                : GetSolutionOrderItem()?.CatalogueItemId;
        }

        public List<CatalogueItemId> GetOrderItemIds()
        {
            var output = new List<CatalogueItemId>();
            var solution = GetSolutionOrderItem();

            if (solution != null)
            {
                output.Add(solution.CatalogueItemId);
            }

            output.AddRange(GetAdditionalServices().Select(x => x.CatalogueItemId));
            output.AddRange(GetAssociatedServices().Select(x => x.CatalogueItemId));

            return output;
        }

        public CatalogueItemId? GetNextOrderItemId(CatalogueItemId current)
        {
            var allIds = GetOrderItemIds();

            if (!allIds.Contains(current))
            {
                return null;
            }

            var index = allIds.IndexOf(current);

            return allIds.Count > (index + 1)
                ? allIds[index + 1]
                : null;
        }

        public CatalogueItemId? GetPreviousOrderItemId(CatalogueItemId current)
        {
            var allIds = GetOrderItemIds();

            if (!allIds.Contains(current))
            {
                return null;
            }

            var index = allIds.IndexOf(current);

            return index > 0
                ? allIds[index - 1]
                : null;
        }

        public OrderItem OrderItem(CatalogueItemId catalogueItemId)
        {
            return OrderItems.FirstOrDefault(x => x.CatalogueItem.Id == catalogueItemId);
        }

        public OrderItem GetSolutionOrderItem()
        {
            return OrderItems
                .FirstOrDefault(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution);
        }

        public IEnumerable<OrderItem> GetSolutions()
        {
            return OrderItems
                .Where(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution)
                .OrderBy(x => x.CatalogueItem.Name);
        }

        public IEnumerable<OrderItem> GetAdditionalServices()
        {
            return OrderItems
                .Where(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService)
                .OrderBy(x => x.CatalogueItem.Name);
        }

        public OrderItem GetAssociatedService(CatalogueItemId catalogueItemId)
        {
            return OrderItems
                .FirstOrDefault(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService
                    && x.CatalogueItem.Id == catalogueItemId);
        }

        public IEnumerable<OrderItem> GetAssociatedServices()
        {
            return OrderItems
                .Where(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService)
                .OrderBy(x => x.CatalogueItem.Name);
        }

        public bool HasAssociatedService()
        {
            return OrderItems.Any(o => o.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService);
        }

        public bool HasValidCatalogueItems()
        {
            if (!IsAmendment)
            {
                return OrderItems.Any(o => o.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution);
            }
            else
            {
                return OrderItems.Any(o => o.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution
                    || o.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService);
            }
        }

        public bool HasSublocationsWithNoRecipients()
        {
            return OrderSublocations is { Count: > 0 }
                && OrderSublocations.Any(x => x.SublocationRecipients.Count == 0);
        }

        public void Apply(Order orderToApply)
        {
            // helps with backwards compatability - if we have an amendment where we didn't copy across all the items.
            foreach (OrderItem orderItemToApply in orderToApply.OrderItems)
            {
                OrderItem currentOrderItem =
                    OrderItems.FirstOrDefault(x => x.CatalogueItemId == orderItemToApply.CatalogueItemId);

                if (currentOrderItem == null)
                {
                    OrderItems.Add(orderItemToApply);
                }
            }

            // Merge recipients on existing sublocations
            foreach (OrderSublocation currentOrderSublocation in OrderSublocations)
            {
                OrderSublocation sublocationToApply =
                    orderToApply.OrderSublocations.FirstOrDefault(x =>
                        x.SublocationOdsCode == currentOrderSublocation.SublocationOdsCode);

                if (sublocationToApply is null)
                {
                    continue;
                }

                IEnumerable<OrderSublocationRecipient> sublocationRecipientsToApply =
                    sublocationToApply.SublocationRecipients.Where(x =>
                        currentOrderSublocation.SublocationRecipients.All(y =>
                            y.RecipientOdsCode != x.RecipientOdsCode));

                foreach (OrderSublocationRecipient newSublocationRecipient in sublocationRecipientsToApply)
                {
                    currentOrderSublocation.SublocationRecipients.Add(newSublocationRecipient);
                }
            }

            IEnumerable<OrderSublocation> sublocationsToApply = orderToApply.OrderSublocations.Where(x =>
                OrderSublocations.All(y => y.SublocationOdsCode != x.SublocationOdsCode));

            foreach (OrderSublocation newSublocation in sublocationsToApply)
            {
                OrderSublocations.Add(newSublocation);
            }

            // Merge order item recipients on each recipient
            foreach (OrderSublocationRecipient recipientToApply in orderToApply.FlattenedRecipients)
            {
                OrderSublocationRecipient existingRecipient = FlattenedRecipients.FirstOrDefault(x =>
                    x.ParentSublocationOdsCode == recipientToApply.ParentSublocationOdsCode
                    && x.RecipientOdsCode == recipientToApply.RecipientOdsCode);

                if (existingRecipient is null)
                {
                    continue;
                }

                foreach (OrderItemSublocationRecipient newOrderItemSublocationRecipient in recipientToApply
                             .OrderItemSublocationRecipients)
                {
                    if (existingRecipient.OrderItemSublocationRecipients.All(x =>
                            x.CatalogueItemId != newOrderItemSublocationRecipient.CatalogueItemId))
                    {
                        existingRecipient.OrderItemSublocationRecipients.Add(newOrderItemSublocationRecipient);
                    }
                }
            }
        }

        public bool Equals(Order other)
        {
            if (ReferenceEquals(null, other))
                return false;

            return ReferenceEquals(this, other) || Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Order);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public Order Clone()
        {
            return new Order
            {
                AssociatedServicesOnlyDetails = AssociatedServicesOnlyDetails,
                DeliveryDate = DeliveryDate,
                Revision = Revision,
                OrderType = OrderType,
                Description = Description,
                OrderItems = OrderItems.Select(x => x.Clone()).ToList(),
                OrderSublocations = OrderSublocations.Select(x => x.Clone()).ToList(),
            };
        }

        public Order BuildAmendment(int newRevision)
        {
            var amendedOrder = new Order
            {
                OrderNumber = OrderNumber,
                Revision = newRevision,
                OrderType = OrderType,
                CommencementDate = CommencementDate,
                Description = Description,
                InitialPeriod = InitialPeriod,
                MaximumTerm = MaximumTerm,
                OrderingPartyId = OrderingPartyId,
                OrderingPartyContact = OrderingPartyContact.Clone(),
                SelectedFrameworkId = SelectedFrameworkId,
                SupplierId = SupplierId,
                SupplierContact = SupplierContact.Clone(),
            };

            amendedOrder.InitialiseOrderItemsFrom(OrderItems);

            amendedOrder.OrderSublocations =
                OrderSublocations.Select(x => x.Clone()).ToList();

            return amendedOrder;
        }

        public void InitialiseOrderItemsFrom(ICollection<OrderItem> items)
        {
            foreach (var item in items)
            {
                if (item.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService) continue;
                var existingOrderItem = OrderItems.FirstOrDefault(x => x.CatalogueItemId == item.CatalogueItemId);

                if (existingOrderItem == null)
                {
                    OrderItems.Add(
                        InitialiseOrderItem(
                            item.CatalogueItem.Id,
                            item.OrderItemPrice?.Clone(),
                            item.Quantity,
                            item.EstimationPeriod));
                }
            }
        }

        public OrderItem InitialiseOrderItem(CatalogueItemId catalogueItemId)
        {
            return new OrderItem { OrderId = Id, CatalogueItemId = catalogueItemId, Created = DateTime.UtcNow };
        }

        public ICollection<OrderSublocationRecipient> DetermineOrderRecipients(
            Order previous,
            CatalogueItemId catalogueItemId)
        {
            if (!Exists(catalogueItemId))
            {
                return [];
            }

            if (previous == null || !previous.Exists(catalogueItemId))
            {
                // No previous order or this order item is new, all recipients apply
                return GetOrderRecipients().ToList();
            }

            // only the new recipients or recipients from previous orders with missing values
            // which might happen if we amend migrated order that wasn't global recipient compatible
            return GetOrderRecipients()
                .Where(PreviousRecipientDidNotExistOrHaveCatalogueItemPredicate(previous, catalogueItemId))
                .ToList();

            // it doesn't exist on this order so no recipients apply
        }

        public bool Exists(CatalogueItemId catalogueItemId)
        {
            return OrderItems.Any(x => x.CatalogueItemId == catalogueItemId);
        }

        private static Func<OrderSublocationRecipient, bool> PreviousRecipientDidNotExistOrHaveCatalogueItemPredicate(
            Order previous,
            CatalogueItemId catalogueItemId)
        {
            return cr =>
            {
                OrderSublocationRecipient previousRecipient =
                    previous.FlattenedRecipients.FirstOrDefault(pr =>
                        pr.RecipientOdsCode == cr.RecipientOdsCode);

                return previousRecipient is null
                    || previousRecipient.OrderItemSublocationRecipients.All(oir =>
                        oir.CatalogueItemId != catalogueItemId);
            };
        }

        private OrderItem InitialiseOrderItem(
            CatalogueItemId catalogueItemId,
            OrderItemPrice orderItemPrice,
            int? quantity,
            TimeUnit? estimationPeriod)
        {
            var orderItem = InitialiseOrderItem(catalogueItemId);
            orderItem.OrderItemPrice = orderItemPrice;
            orderItem.Quantity = quantity;
            orderItem.EstimationPeriod = estimationPeriod;
            return orderItem;
        }

        private bool HaveAllQuantities(ICollection<OrderSublocationRecipient> orderRecipients)
        {
            var recipients = orderRecipients.ToList();

            return OrderItems.All(recipients.AllQuantitiesEntered);
        }
    }
}
