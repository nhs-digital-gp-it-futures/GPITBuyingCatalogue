using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class Epic : IAudited
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int CapabilityId { get; set; }

        public string SourceUrl { get; set; }

        public bool IsActive { get; set; }

        public bool SupplierDefined { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public CompliancyLevel CompliancyLevel { get; set; }

        public Capability Capability { get; set; }
    }
}
