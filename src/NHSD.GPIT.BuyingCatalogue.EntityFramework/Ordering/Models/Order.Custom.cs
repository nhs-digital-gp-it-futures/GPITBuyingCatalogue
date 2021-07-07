using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class Order
    {
        private readonly List<DefaultDeliveryDate> defaultDeliveryDates = new();
        private readonly List<OrderItem> orderItems = new();
        private readonly List<ServiceInstanceItem> serviceInstanceItems = new();

        private DateTime lastUpdated;
        private Guid lastUpdatedBy;
        private string lastUpdatedByName;
        private DateTime? completed;

        public IReadOnlyList<ServiceInstanceItem> ServiceInstanceItems => serviceInstanceItems.AsReadOnly();

        public decimal CalculateCostPerYear(CostType costType)
        {
            return OrderItems.Where(i => i.CostType == costType).Sum(i => i.CalculateTotalCostPerYear());
        }

        public decimal CalculateTotalOwnershipCost()
        {
            const int defaultContractLength = 3;

            return CalculateCostPerYear(CostType.OneOff) + (defaultContractLength * CalculateCostPerYear(CostType.Recurring));
        }

        public void Complete()
        {
            // TODO - review error handling in OAPI
            OrderStatus = OrderStatus.Complete;
            completed = DateTime.UtcNow;
        }

        public DeliveryDateResult SetDefaultDeliveryDate(CatalogueItemId catalogueItemId, DateTime date)
        {
            var result = DeliveryDateResult.Updated;

            var existingDate = DefaultDeliveryDates.SingleOrDefault(d => d.CatalogueItemId == catalogueItemId);
            if (existingDate is null)
            {
                existingDate = new DefaultDeliveryDate
                {
                    CatalogueItemId = catalogueItemId,
                    OrderId = Id,
                };

                DefaultDeliveryDates.Add(existingDate);
                result = DeliveryDateResult.Added;
            }

            existingDate.DeliveryDate = date;
            return result;
        }

        public void SetLastUpdatedBy(Guid userId, string userName)
        {
            lastUpdatedBy = userId;
            lastUpdatedByName = userName ?? throw new ArgumentNullException(nameof(userName));
            lastUpdated = completed ?? DateTime.UtcNow;
        }

        public OrderItem AddOrUpdateOrderItem(OrderItem orderItem)
        {
            if (orderItem is null)
                throw new ArgumentNullException(nameof(orderItem));

            // Included to force EF Core to track order as changed so audit information is updated
            lastUpdated = DateTime.UtcNow;

            var existingItem = orderItems.SingleOrDefault(o => o.Equals(orderItem));
            if (existingItem is null)
            {
                orderItems.Add(orderItem);
                orderItem.MarkOrderSectionAsViewed(this);

                return orderItem;
            }

            existingItem.EstimationPeriod = orderItem.EstimationPeriod;
            existingItem.PriceId = orderItem.PriceId;
            existingItem.Price = orderItem.Price;

            return existingItem;
        }

        public bool CanComplete()
        {
            return
                !string.IsNullOrWhiteSpace(Description)
                && OrderingPartyContact is not null
                && Supplier is not null
                && CommencementDate is not null
                && (HasSolution() || HasAssociatedService())
                && FundingSourceOnlyGms.HasValue
                && OrderStatus != OrderStatus.Complete;
        }

        public void DeleteOrderItemAndUpdateProgress(CatalogueItemId catalogueItemId)
        {
            orderItems.RemoveAll(o => o.CatalogueItem.CatalogueItemId == catalogueItemId
                || o.CatalogueItem.AdditionalService?.SolutionId == catalogueItemId);

            if (!HasSolution())
            {
                Progress.AdditionalServicesViewed = false;
            }

            if (orderItems.Count == 0)
            {
                FundingSourceOnlyGms = null;
            }
        }

        public bool HasAssociatedService()
        {
            return OrderItems.Any(o => o.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService);
        }

        public bool HasSolution()
        {
            return OrderItems.Any(o => o.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution);
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
    }
}
