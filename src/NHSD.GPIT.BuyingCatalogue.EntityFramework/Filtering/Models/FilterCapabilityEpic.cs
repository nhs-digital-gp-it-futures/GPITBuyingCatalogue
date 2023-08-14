using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models
{
    public sealed class FilterCapabilityEpic
    {
        public int Id { get; set; }

        public int FilterId { get; set; }

        public int? CapabilityId { get; set; }

        public string EpicId { get; set; }

        public Capability Capability { get; set; }

        public Epic Epic { get; set; }
    }
}
