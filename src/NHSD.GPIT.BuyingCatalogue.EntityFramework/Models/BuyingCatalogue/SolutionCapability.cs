using System;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class SolutionCapability
    {
        public string SolutionId { get; set; }

        public Guid CapabilityId { get; set; }

        public int StatusId { get; set; }

        public DateTime LastUpdated { get; set; }

        public Guid LastUpdatedBy { get; set; }

        public virtual Capability Capability { get; set; }

        public virtual Solution Solution { get; set; }

        public virtual SolutionCapabilityStatus Status { get; set; }
    }
}
