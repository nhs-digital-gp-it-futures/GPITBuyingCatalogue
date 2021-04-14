using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
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
