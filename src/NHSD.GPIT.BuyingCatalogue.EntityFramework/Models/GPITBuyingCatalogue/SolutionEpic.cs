using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public partial class SolutionEpic
    {
        public CatalogueItemId SolutionId { get; set; }

        public Guid CapabilityId { get; set; }

        public string EpicId { get; set; }

        public int StatusId { get; set; }

        public DateTime LastUpdated { get; set; }

        public Guid LastUpdatedBy { get; set; }

        public virtual Capability Capability { get; set; }

        public virtual Epic Epic { get; set; }

        public virtual Solution Solution { get; set; }

        public virtual SolutionEpicStatus Status { get; set; }
    }
}
