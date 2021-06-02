using System;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public partial class FrameworkCapability
    {
        public string FrameworkId { get; set; }

        public Guid CapabilityId { get; set; }

        public bool IsFoundation { get; set; }

        public virtual Capability Capability { get; set; }

        public virtual Framework Framework { get; set; }
    }
}
