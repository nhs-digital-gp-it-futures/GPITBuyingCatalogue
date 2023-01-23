using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class StandardCapability : IAudited
    {
        public StandardCapability()
        {
        }

        public StandardCapability(string standardId, int capabilityId)
        {
            StandardId = standardId;
            CapabilityId = capabilityId;
        }

        public string StandardId { get; set; }

        public int CapabilityId { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public Standard Standard { get; set; }

        public Capability Capability { get; set; }
    }
}
