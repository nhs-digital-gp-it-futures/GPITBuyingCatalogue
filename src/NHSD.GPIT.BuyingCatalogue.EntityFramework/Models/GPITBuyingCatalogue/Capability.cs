using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public partial class Capability
    {
        public Capability()
        {
            Epics = new HashSet<Epic>();
            FrameworkCapabilities = new HashSet<FrameworkCapability>();
            SolutionCapabilities = new HashSet<SolutionCapability>();
            SolutionEpics = new HashSet<SolutionEpic>();
        }

        public Guid Id { get; set; }

        public string CapabilityRef { get; set; }

        public string Version { get; set; }

        public string PreviousVersion { get; set; }

        public int StatusId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string SourceUrl { get; set; }

        public DateTime EffectiveDate { get; set; }

        public int CategoryId { get; set; }

        public virtual CapabilityCategory Category { get; set; }

        public virtual CapabilityStatus Status { get; set; }

        public virtual ICollection<Epic> Epics { get; set; }

        public virtual ICollection<FrameworkCapability> FrameworkCapabilities { get; set; }

        public virtual ICollection<SolutionCapability> SolutionCapabilities { get; set; }

        public virtual ICollection<SolutionEpic> SolutionEpics { get; set; }
    }
}
