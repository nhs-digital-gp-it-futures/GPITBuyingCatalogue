using System;
using System.Collections.Generic;

#nullable disable

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public sealed class Epic
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

        public bool SupplierDefined { get; set; }

        public Capability Capability { get; set; }

        public CompliancyLevel CompliancyLevel { get; set; }

        public ICollection<SolutionEpic> SolutionEpics { get; set; }
    }
}
