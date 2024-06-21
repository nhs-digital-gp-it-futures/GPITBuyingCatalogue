using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models
{
    public sealed class FilterInteroperabilityIntegrationType
    {
        public int FilterId { get; set; }

        public int Id { get; set; }

        public SupportedIntegrations InteroperabilityIntegrationType { get; set; }
    }
}
