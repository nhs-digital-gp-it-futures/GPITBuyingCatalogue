using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class SolutionCapabilityStatus
    {
        public SolutionCapabilityStatus()
        {
            SolutionCapabilities = new HashSet<SolutionCapability>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Pass { get; set; }

        public virtual ICollection<SolutionCapability> SolutionCapabilities { get; set; }
    }
}
