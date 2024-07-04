using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models
{
    public sealed class FilterHostingType
    {
        public int FilterId { get; set; }

        public int Id { get; set; }

        public HostingType HostingType { get; set; }
    }
}
