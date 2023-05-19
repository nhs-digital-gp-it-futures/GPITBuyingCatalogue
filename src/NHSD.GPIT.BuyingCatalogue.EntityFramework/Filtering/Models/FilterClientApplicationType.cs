using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models
{
    public sealed class FilterClientApplicationType
    {
        public int Id { get; set; }

        public int FilterId { get; set; }

        public ClientApplicationType ClientApplicationType { get; set; }
    }
}
