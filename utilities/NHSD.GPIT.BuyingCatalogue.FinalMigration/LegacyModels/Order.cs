using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.FinalMigration.LegacyModels
{
    public partial class Order
    {
        public int Id { get; set; }
        public byte Revision { get; set; }
        public string CallOffId { get; set; }
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
        public int OrderStatusId { get; set; }
        public bool IsDeleted { get; set; }

        public int NewId { get; set; }
    }
}
