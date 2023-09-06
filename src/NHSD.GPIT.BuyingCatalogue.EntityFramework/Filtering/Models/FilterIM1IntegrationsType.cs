using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models
{
    public sealed class FilterIM1IntegrationsType
    {
        public int FilterId { get; set; }

        public int Id { get; set; }

        public InteropIm1Integrations IM1IntegrationsType { get; set; }
    }
}
