using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public partial class Order
    {
        public const string LocalFunding = "Local";
        public const string CentralFunding = "Central";

        // TODO: remove with csv
        public string ApproximateFundingType
        {
            get
            {
                var fundingTypes = OrderItems
                    .Select(x => x.FundingType)
                    .Where(x => x != OrderItemFundingType.NoFundingRequired
                        && x != OrderItemFundingType.None)
                    .ToList();

                if (!fundingTypes.Any())
                {
                    return CentralFunding;
                }

                return fundingTypes.All(x => x is OrderItemFundingType.LocalFunding or OrderItemFundingType.LocalFundingOnly)
                    ? LocalFunding
                    : CentralFunding;
            }
        }

        public string EndDateDisplayValue => new EndDate(CommencementDate, MaximumTerm).DisplayValue;

        public EndDate EndDate => new EndDate(CommencementDate, MaximumTerm, OrderTriageValue);

        public bool IsAmendment => CallOffId.IsAmendment;

        public void Complete()
        {
            Completed = DateTime.UtcNow;
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
                && OrderItems.All(x => x.OrderItemRecipients.All(r => r.DeliveryDate != null))
                && OrderItems.All(oi => oi.OrderItemFunding is not null)
                && OrderStatus != OrderStatus.Completed;
        }

        public CatalogueItemId? GetSolutionId()
        {
            return AssociatedServicesOnly
                ? SolutionId
                : GetSolution()?.CatalogueItemId;
        }

        public List<CatalogueItemId> GetOrderItemIds()
        {
            var output = new List<CatalogueItemId>();
            var solution = GetSolution();

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

        public void Apply(Order order)
        {
            foreach (var orderItem in order.OrderItems)
            {
                var existing = OrderItems.FirstOrDefault(x => x.CatalogueItemId == orderItem.CatalogueItemId);

                if (existing == null)
                {
                    OrderItems.Add(orderItem);
                }
                else
                {
                    foreach (var recipient in orderItem.OrderItemRecipients)
                    {
                        if (existing.OrderItemRecipients.All(x => x.OdsCode != recipient.OdsCode))
                        {
                            existing.OrderItemRecipients.Add(recipient);
                        }
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
            var inputSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
            };

            var outputSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects,
            };

            var serialised = JsonConvert.SerializeObject(this, inputSettings);
            var output = JsonConvert.DeserializeObject<Order>(serialised, outputSettings);

            return output;
        }

        public Order BuidAmendment(int newRevision)
        {
            return new Order
            {
                OrderNumber = OrderNumber,
                Revision = newRevision,
                AssociatedServicesOnly = AssociatedServicesOnly,
                CommencementDate = CommencementDate,
                Description = Description,
                InitialPeriod = InitialPeriod,
                MaximumTerm = MaximumTerm,
                OrderingPartyId = OrderingPartyId,
                OrderingPartyContact = OrderingPartyContact.Clone(),
                OrderTriageValue = OrderTriageValue,
                SelectedFrameworkId = SelectedFrameworkId,
                SupplierId = SupplierId,
                SupplierContact = SupplierContact.Clone(),
            };
        }
    }
}
