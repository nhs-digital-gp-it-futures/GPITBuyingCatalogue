using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed class FilterCapability : IAudited
    {
        public FilterCapability()
        {
        }

        public FilterCapability(
            string filterId,
            int capabilityId)
        {
            FilterId = filterId;
            CapabilityId = capabilityId;
        }

        public string FilterId { get; set; }

        public int CapabilityId { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public Capability Capability { get; set; }

        public Filter Filter { get; set; }
    }
}
