﻿using System;
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
            DefaultDeliveryDates = new HashSet<DefaultDeliveryDate>();
        }

        public int Id { get; set; }

        [NotMapped]
        public CallOffId CallOffId => new(Id, 1);

        public string Description { get; set; }

        // TODO: remove
        public int OrderingPartyId { get; set; }

        public int? OrderingPartyContactId { get; set; }

        // TODO: remove
        public int? SupplierId { get; set; }

        public int? SupplierContactId { get; set; }

        public DateTime? CommencementDate { get; set; }

        public bool? FundingSourceOnlyGms { get; set; }

        public DateTime Created { get; set; } = DateTime.UtcNow;

        public DateTime LastUpdated => lastUpdated;

        public int LastUpdatedBy => lastUpdatedBy;

        public AspNetUser LastUpdatedByUser { get; set; }

        public string LastUpdatedByName => $"{LastUpdatedByUser.FirstName} {LastUpdatedByUser.LastName}";

        public DateTime? Completed => completed;

        public bool IsDeleted { get; set; }

        public OrderStatus OrderStatus { get; set; } = OrderStatus.Incomplete;

        public Organisation OrderingParty { get; set; }

        public Contact OrderingPartyContact { get; set; }

        public Supplier Supplier { get; set; }

        public Contact SupplierContact { get; set; }

        public ICollection<DefaultDeliveryDate> DefaultDeliveryDates { get; set; }

        public IReadOnlyList<OrderItem> OrderItems => orderItems.AsReadOnly();
    }
}
