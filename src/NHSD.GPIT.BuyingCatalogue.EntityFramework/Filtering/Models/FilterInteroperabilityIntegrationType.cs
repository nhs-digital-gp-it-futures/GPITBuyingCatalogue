using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models
{
    public sealed class FilterInteroperabilityIntegrationType
    {
        public int FilterId { get; set; }

        public int Id { get; set; }

        public InteropIntegrationType InteroperabilityIntegrationType { get; set; }
    }
}
