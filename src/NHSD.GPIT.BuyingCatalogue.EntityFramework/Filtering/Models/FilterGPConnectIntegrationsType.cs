using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models
{
    public sealed class FilterGPConnectIntegrationsType
    {
        public int FilterId { get; set; }

        public int Id { get; set; }

        public InteropGpConnectIntegrations GPConnectIntegrationsType { get; set; }
    }
}
