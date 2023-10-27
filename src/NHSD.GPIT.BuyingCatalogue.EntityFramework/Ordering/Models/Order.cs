using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Newtonsoft.Json;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    [Serializable]
    public partial class Order : IAudited
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
            OrderRecipients = new HashSet<OrderRecipient>();
        }

        public int Id { get; set; }

        public int OrderNumber { get; set; }

        public int Revision { get; set; }

        [NotMapped]
        [JsonIgnore]
        public CallOffId CallOffId => new(OrderNumber, Revision);

        public string Description { get; set; }

        public int OrderingPartyId { get; set; }

        public int? OrderingPartyContactId { get; set; }

        public int? SupplierId { get; set; }

        public int? SupplierContactId { get; set; }

        public DateTime? CommencementDate { get; set; }

        public int? InitialPeriod { get; set; }

        public int? MaximumTerm { get; set; }

        public OrderType OrderType { get; set; } = OrderTypeEnum.Unknown;

        [Obsolete("Work in progress. References will be evaluated and changed to use OrderType")]
        public bool AssociatedServicesOnly => OrderType.Value == OrderTypeEnum.AssociatedServiceOther;

        public OrderTriageValue? OrderTriageValue { get; set; }

        public string SelectedFrameworkId { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public DateTime? Completed { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsTerminated { get; set; }

        public virtual OrderDeletionApproval OrderDeletionApproval { get; set; }

        public virtual OrderTermination OrderTermination { get; set; }

        [NotMapped]
        [JsonIgnore]
        public OrderStatus OrderStatus
        {
            get
            {
                if (IsTerminated)
                    return OrderStatus.Terminated;

                if (IsDeleted)
                    return OrderStatus.Deleted;

                return Completed.HasValue
                    ? OrderStatus.Completed
                    : OrderStatus.InProgress;
            }
        }

        public Organisation OrderingParty { get; set; }

        public Contact OrderingPartyContact { get; set; }

        public Supplier Supplier { get; set; }

        public Contact SupplierContact { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }

        public ICollection<OrderRecipient> OrderRecipients { get; set; }

        public AssociatedServicesOnlyDetails AssociatedServicesOnlyDetails { get; set; }

        public Framework SelectedFramework { get; set; }

        public virtual ContractFlags ContractFlags { get; set; }

        public virtual Contract Contract { get; set; }

        public IEnumerable<CatalogueItem> GetServices(CatalogueItemType catalogueItemType)
        {
            return catalogueItemType switch
            {
                CatalogueItemType.AdditionalService => GetAdditionalServices().Select(x => x.CatalogueItem).ToList(),
                CatalogueItemType.AssociatedService => GetAssociatedServices().Select(x => x.CatalogueItem).ToList(),
                _ => throw new ArgumentOutOfRangeException(nameof(catalogueItemType), catalogueItemType, null),
            };
        }
    }
}
