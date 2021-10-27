using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class SlaServiceLevels
    {
        public int Id { get; set; }

        public string TypeOfService { get; set; }

        public string ServiceLevel { get; set; }

        public string HowMeasured { get; set; }

        public bool ServiceCredits { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public ServiceLevelAgreements ServiceLevelAgreement { get; set; }
    }
}
