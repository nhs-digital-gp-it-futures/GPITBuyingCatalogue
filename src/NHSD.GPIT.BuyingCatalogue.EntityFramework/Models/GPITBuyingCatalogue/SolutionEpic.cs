using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue
{
    public sealed class SolutionEpic
    {
        public CatalogueItemId SolutionId { get; set; }

        public Guid CapabilityId { get; set; }

        public string EpicId { get; set; }

        public int StatusId { get; set; }

        public DateTime LastUpdated { get; set; }

        public Guid LastUpdatedBy { get; set; }

        public Epic Epic { get; set; }

        public SolutionEpicStatus Status { get; set; }
    }
}
