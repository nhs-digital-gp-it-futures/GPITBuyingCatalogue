using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public class SolutionCapability
    {
        public CatalogueItemId SolutionId { get; set; }

        public Guid CapabilityId { get; set; }

        public int StatusId { get; set; }

        public DateTime LastUpdated { get; set; }

        public Guid LastUpdatedBy { get; set; }

        public virtual Capability Capability { get; set; }

        public virtual SolutionCapabilityStatus Status { get; set; }
    }
}
