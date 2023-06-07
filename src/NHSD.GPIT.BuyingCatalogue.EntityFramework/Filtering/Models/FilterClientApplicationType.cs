using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models
{
    public sealed class FilterClientApplicationType
    {
        public int Id { get; set; }

        public int FilterId { get; set; }

        public ApplicationType ApplicationType { get; set; }
    }
}
