using System;
using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class Order
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

            existingItem.PriceTimeUnit = orderItem.PriceTimeUnit;
            existingItem.EstimationPeriod = orderItem.EstimationPeriod;
            existingItem.PriceId = orderItem.PriceId;
            existingItem.Price = orderItem.Price;

            return existingItem;
        }

        public bool CanComplete()
        {
            if (!FundingSourceOnlyGms.HasValue)
                return false;

            int catalogueSolutionsCount = OrderItems.Count(o => o.CatalogueItem.CatalogueItemType == CatalogueItemType.Solution);
            int associatedServicesCount = OrderItems.Count(o => o.CatalogueItem.CatalogueItemType == CatalogueItemType.AssociatedService);

            var solutionAndAssociatedServices = catalogueSolutionsCount > 0
                && associatedServicesCount > 0;

            var solutionAndNoAssociatedServices = catalogueSolutionsCount > 0
                && associatedServicesCount == 0
                && OrderProgress.AssociatedServicesViewed;

            var noSolutionsAndAssociatedServices = catalogueSolutionsCount == 0
                && OrderProgress.CatalogueSolutionsViewed
                && associatedServicesCount > 0;

            return solutionAndNoAssociatedServices
                || solutionAndAssociatedServices
                || noSolutionsAndAssociatedServices;
        }

        public int DeleteOrderItemAndUpdateProgress(CatalogueItemId catalogueItemId)
        {
            var result = orderItems.RemoveAll(o => o.CatalogueItem.Id == catalogueItemId
                || o.CatalogueItem.ParentCatalogueItemId == catalogueItemId);

            if (!HasSolution())
            {
                OrderProgress.AdditionalServicesViewed = false;
            }

            if (orderItems.Count == 0)
            {
                FundingSourceOnlyGms = null;
            }

            return result;
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
