using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public partial class Framework
    {
        public Framework()
        {
            FrameworkCapabilities = new HashSet<FrameworkCapability>();
            FrameworkSolutions = new HashSet<FrameworkSolution>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string ShortName { get; set; }

        public string Description { get; set; }

        public string Owner { get; set; }

        public DateTime? ActiveDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public virtual ICollection<FrameworkCapability> FrameworkCapabilities { get; set; }

        public virtual ICollection<FrameworkSolution> FrameworkSolutions { get; set; }
    }
}
