using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Ordering
{
    public partial class Order
    {
        public Order()
        {
            DefaultDeliveryDates = new HashSet<DefaultDeliveryDate>();
            OrderItems = new HashSet<OrderItem>();
            SelectedServiceRecipients = new HashSet<SelectedServiceRecipient>();
        }

        public int Id { get; set; }

        public byte Revision { get; set; }

        public CallOffId CallOffId { get; set; }

        public string Description { get; set; }

        public Guid OrderingPartyId { get; set; }

        public int? OrderingPartyContactId { get; set; }

        public string SupplierId { get; set; }

        public int? SupplierContactId { get; set; }

        public DateTime? CommencementDate { get; set; }

        public bool? FundingSourceOnlyGms { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public Guid LastUpdatedBy { get; set; }

        public string LastUpdatedByName { get; set; }

        public DateTime? Completed { get; set; }

        public bool IsDeleted { get; set; }

        public virtual OrderStatus OrderStatus { get; set; } = OrderStatus.Incomplete;

        public virtual OrderingParty OrderingParty { get; set; }

        public virtual Contact OrderingPartyContact { get; set; }

        public virtual Supplier Supplier { get; set; }

        public virtual Contact SupplierContact { get; set; }

        public virtual OrderProgress OrderProgress { get; set; }

        public virtual ICollection<DefaultDeliveryDate> DefaultDeliveryDates { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; }

        public virtual ICollection<SelectedServiceRecipient> SelectedServiceRecipients { get; set; }
    }
}
