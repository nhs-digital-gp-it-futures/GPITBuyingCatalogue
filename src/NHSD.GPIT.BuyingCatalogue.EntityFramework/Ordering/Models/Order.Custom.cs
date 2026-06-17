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
            ICollection<OrderItem> orderItems,
            Order previous = null)
        {
            return
                !string.IsNullOrWhiteSpace(Description)
                && OrderingPartyContact is not null
                && Supplier is not null
                && CommencementDate is not null
                && (HasValidCatalogueItems() || HasAssociatedService())
                && !HasSublocationsWithNoRecipients()
                && OrderItems.Count > 0
                && HaveAllDeliveryDates(orderRecipients, previous)
                && AllValuesEntered(orderRecipients, (recipients, item) => recipients.AllQuantitiesEntered(item), previous)
                && orderItems.All(oi => oi.OrderItemFunding is not null)
                && ContractFlags is not null
                && (!OrderType.ImplementationPlanRequired || Contract?.ImplementationPlan is not null)
                && (IsAmendment || !HasAssociatedService() || Contract?.ContractBilling is not null)
                && ContractFlags?.UseDefaultDataProcessing == true
                && AcceptedTermsAndConditions
                && OrderStatus == OrderStatus.InProgress;
        }

        public bool HaveAllDeliveryDates(ICollection<OrderSublocationRecipient> orderRecipients, Order previous = null)
        {
            return AllValuesEntered(
                orderRecipients,
                (recipients, item) => recipients.AllDeliveryDatesEntered(item.Id),
                previous);
        }

        public CatalogueItemId? GetSolutionId()
        {
            return OrderType.AssociatedServicesOnly
                ? AssociatedServicesOnlyDetails.SolutionId
                : GetSolutionOrderItem()?.CatalogueItemId;
        }

        public List<int> GetOrderItemIds()
        {
            var output = new List<int>();
            var solution = GetSolutionOrderItem();

            if (solution != null)
            {
                output.Add(solution.Id);
            }

            output.AddRange(GetAdditionalServices().SelectMany(additionalService =>
            {
                var result = new List<int> { additionalService.Id };
                result.AddRange(additionalService.Services.Select(associatedService => associatedService.Id));
                return result;
            }));
            output.AddRange(GetAssociatedServices().Select(x => x.Id));

            return output;
        }

        public int? GetNextOrderItemId(int current)
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

        public int? GetPreviousOrderItemId(int current)
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

        public OrderItem OrderItem(int orderItemId)
        {
            return OrderItems.FirstOrDefault(item => item.Id == orderItemId);
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
                .Where(x => x.CatalogueItem?.CatalogueItemType == CatalogueItemType.AdditionalService)
                .OrderBy(x => x.CatalogueItem.Name);
        }

        public IEnumerable<OrderItem> GetAllAssociatedServices()
        {
            return OrderItems
                .Where(item => item.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService);
        }

        public OrderItem GetAssociatedService(int orderItemId)
        {
            return OrderItems
                .FirstOrDefault(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService
                    && x.Id == orderItemId);
        }

        public IEnumerable<OrderItem> GetAssociatedServices()
        {
            return OrderItems
                .Where(x => (x.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService)
                    && (GetSolutionOrderItem() == null || x.ParentId == GetSolutionOrderItem().Id))
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
                            x.OrderItemId != newOrderItemSublocationRecipient.OrderItemId))
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
                        InitialiseAmendOrderItem(
                            item.CatalogueItem.Id,
                            item.OrderItemPrice?.Clone(),
                            item.EstimationPeriod));
                }
            }

            return;

            OrderItem InitialiseAmendOrderItem(
                CatalogueItemId catalogueItemId,
                OrderItemPrice orderItemPrice,
                TimeUnit? estimationPeriod)
            {
                var orderItem = InitialiseOrderItem(catalogueItemId);
                if (orderItemPrice is { CataloguePriceCalculationType: CataloguePriceCalculationType.SingleFixed })
                {
                    orderItemPrice.PriceTiers.ToList().ForEach(tier => tier.Price = 0m);
                }

                orderItem.OrderItemPrice = orderItemPrice;
                orderItem.EstimationPeriod = estimationPeriod;
                return orderItem;
            }
        }

        public OrderItem InitialiseOrderItem(CatalogueItemId catalogueItemId, int? parentId = null)
        {
            return new OrderItem { OrderId = Id, CatalogueItemId = catalogueItemId, Created = DateTime.UtcNow, ParentId = parentId };
        }

        public ICollection<OrderSublocationRecipient> DetermineOrderRecipients(
            Order previous,
            int orderItemId)
        {
            var orderItem = OrderItems.FirstOrDefault(item => item.Id == orderItemId);

            if (orderItem is null || !Exists(orderItem.Id))
            {
                return [];
            }

            if (previous == null || (!previous.Exists(orderItem.Id)
                && orderItem.CatalogueItem.CatalogueItemType != CatalogueItemType.AssociatedService))
            {
                // No previous order or this order item is new, all recipients apply
                return GetOrderRecipients().ToList();
            }

            // only the new recipients or recipients from previous orders with missing values
            // which might happen if we amend migrated order that wasn't global recipient compatible
            return GetOrderRecipients()
                .Where(PreviousRecipientDidNotExistOrHaveCatalogueItemPredicate(previous, orderItemId))
                .Where(CurrentRecipientDidNotExistInPreviousOrderPredicate(previous, IsAmendment))
                .ToList();

            // it doesn't exist on this order so no recipients apply
        }

        public bool Exists(int orderItemId)
        {
            return OrderItems.Any(x => x.Id == orderItemId);
        }

        private static Func<OrderSublocationRecipient, bool> PreviousRecipientDidNotExistOrHaveCatalogueItemPredicate(
            Order previous,
            int orderItemId)
        {
            return cr =>
            {
                OrderSublocationRecipient previousRecipient =
                    previous?.FlattenedRecipients.FirstOrDefault(pr =>
                        pr.RecipientOdsCode == cr.RecipientOdsCode);

                return previousRecipient is null
                    || previousRecipient.OrderItemSublocationRecipients.All(oir =>
                        oir.OrderItem.Id != orderItemId);
            };
        }

        private static Func<OrderSublocationRecipient, bool> CurrentRecipientDidNotExistInPreviousOrderPredicate(
            Order previous,
            bool isAmendment)
        {
            return cr => previous == null || (isAmendment
                && previous.FlattenedRecipients.All(previousRecipient => previousRecipient.RecipientOdsCode != cr.RecipientOdsCode));
        }

        private bool AllValuesEntered(
            ICollection<OrderSublocationRecipient> orderRecipients,
            Func<ICollection<OrderSublocationRecipient>, OrderItem, bool> allValuesPred,
            Order previous = null)
        {
            if (previous is not null && IsAmendment && HasAssociatedService())
            {
                return OrderItems.All(item =>
                {
                    var recipients = DetermineOrderRecipients(previous, item.Id);
                    return allValuesPred(recipients, item);
                });
            }

            return OrderItems.All(item => allValuesPred(orderRecipients, item));
        }
    }
}
