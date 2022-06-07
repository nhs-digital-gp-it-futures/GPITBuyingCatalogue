using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models
{
    public sealed partial class Order : IAudited
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }

        [NotMapped]
        public CallOffId CallOffId => new(Id, 1);

        public string Description { get; set; }

        public int OrderingPartyId { get; set; }

        public int? OrderingPartyContactId { get; set; }

        public int? SupplierId { get; set; }

        public int? SupplierContactId { get; set; }

        public DateTime? CommencementDate { get; set; }

        public int? InitialPeriod { get; set; }

        public int? MaximumTerm { get; set; }

        public bool AssociatedServicesOnly { get; set; }

        public OrderTriageValue? OrderTriageValue { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public DateTime? Completed => completed;

        public bool IsDeleted { get; set; }

        public OrderStatus OrderStatus { get; set; } = OrderStatus.InProgress;

        public Organisation OrderingParty { get; set; }

        public Contact OrderingPartyContact { get; set; }

        public Supplier Supplier { get; set; }

        public Contact SupplierContact { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
