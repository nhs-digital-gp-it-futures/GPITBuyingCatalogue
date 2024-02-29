using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.AdditionalServices
{
    public sealed class AdditionalServicesDetailsModel
    {
        public CatalogueItemId Id { get; set; }

        public string Name { get; init; }

        public string Description { get; init; }

        public int UserId { get; init; }
    }
}
