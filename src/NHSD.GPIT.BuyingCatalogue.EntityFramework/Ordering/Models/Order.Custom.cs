using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class Order
    {
        private readonly List<ServiceInstanceItem> serviceInstanceItems = new();

        private DateTime? completed;

        public IReadOnlyList<ServiceInstanceItem> ServiceInstanceItems => serviceInstanceItems.AsReadOnly();

        // TODO: remove with csv
        public string ApproximateFundingType
        {
            get
            {
                return OrderItems.All(x => x.FundingType is OrderItemFundingType.LocalFunding or OrderItemFundingType.LocalFundingOnly)
                    ? "Local"
                    : "Central";
            }
        }

        public string EndDate => CommencementDate.HasValue && MaximumTerm.HasValue
            ? $"{CommencementDate?.AddMonths(MaximumTerm.Value).AddDays(-1):dd MMMM yyyy}"
            : string.Empty;

        public void Complete()
        {
            OrderStatus = OrderStatus.Completed;
            completed = DateTime.UtcNow;
        }

        public bool CanComplete()
        {
            return
                !string.IsNullOrWhiteSpace(Description)
                && OrderingPartyContact is not null
                && Supplier is not null
                && CommencementDate is not null
                && (HasSolution() || HasAssociatedService())
                && OrderItems.Count > 0
                && OrderItems.All(oi => oi.OrderItemFunding is not null)
                && OrderStatus != OrderStatus.Completed;
        }

        public CatalogueItemId? GetSolutionId()
        {
            return AssociatedServicesOnly
                ? SolutionId
                : GetSolution()?.CatalogueItemId;
        }

        public OrderItem OrderItem(CatalogueItemId catalogueItemId)
        {
            return OrderItems.SingleOrDefault(x => x.CatalogueItem.Id == catalogueItemId);
        }

        public OrderItem GetSolution()
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

        public OrderItem GetAdditionalService(CatalogueItemId catalogueItemId)
        {
            return OrderItems
                .FirstOrDefault(x => x.CatalogueItem.CatalogueItemType == CatalogueItemType.AdditionalService
                    && x.CatalogueItem.Id == catalogueItemId);
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
