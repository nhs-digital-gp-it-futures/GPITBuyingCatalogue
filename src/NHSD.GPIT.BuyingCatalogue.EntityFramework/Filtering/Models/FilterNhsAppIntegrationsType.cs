using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models
{
    public sealed class FilterNhsAppIntegrationsType
    {
        public int FilterId { get; set; }

        public int Id { get; set; }

        public InteropNhsAppIntegrationType NhsAppIntegrationsType { get; set; }
    }
}
