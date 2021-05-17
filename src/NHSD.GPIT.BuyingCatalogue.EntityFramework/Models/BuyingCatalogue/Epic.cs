using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.BuyingCatalogue
{
    public partial class Epic
    {
        public Epic()
        {
            SolutionEpics = new HashSet<SolutionEpic>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public Guid CapabilityId { get; set; }

        public string SourceUrl { get; set; }

        public int? CompliancyLevelId { get; set; }

        public bool Active { get; set; }

        public virtual Capability Capability { get; set; }

        public virtual CompliancyLevel CompliancyLevel { get; set; }

        public virtual ICollection<SolutionEpic> SolutionEpics { get; set; }
    }
}
