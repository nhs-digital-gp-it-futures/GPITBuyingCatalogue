using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public partial class Order
    {
        public const string LocalFunding = "Local";
        public const string CentralFunding = "Central";

        [JsonIgnore]
        public bool IsLocalFundingOnly =>
            SelectedFramework.LocalFundingOnly || OrderingParty.OrganisationType == OrganisationType.GP;

        [JsonIgnore]
        public EndDate EndDate => new(CommencementDate, MaximumTerm);

        [JsonIgnore]
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
                && (HasValidCatalogueItems() || HasAssociatedService())
                && OrderItems.Count > 0
                && OrderItems.All(x => x.OrderItemRecipients.All(r => r.DeliveryDate != null))
                && OrderItems.All(oi => oi.OrderItemFunding is not null)
                && ContractFlags is not null
                && (AssociatedServicesOnly || Contract?.ImplementationPlan is not null)
                && (IsAmendment || !HasAssociatedService() || Contract?.ContractBilling is not null)
                && ContractFlags?.UseDefaultDataProcessing == true
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

        public Order BuildAmendment(int newRevision)
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

        public OrderItem InitialiseOrderItem(CatalogueItem catalogueItem)
        {
            return new OrderItem
            {
                OrderId = Id,
                CatalogueItemId = catalogueItem.Id,
                CatalogueItem = catalogueItem,
                Created = DateTime.UtcNow,
            };
        }

        public OrderItem InitialiseOrderItem(CatalogueItem catalogueItem, OrderItemPrice orderItemPrice)
        {
            var orderItem = InitialiseOrderItem(catalogueItem);
            orderItem.OrderItemPrice = orderItemPrice;
            return orderItem;
        }
    }
}
