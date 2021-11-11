using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class SlaServiceLevel : IAudited
    {
        public int Id { get; set; }

        public string TypeOfService { get; set; }

        public string ServiceLevel { get; set; }

        public string HowMeasured { get; set; }

        public bool ServiceCredits { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public ServiceLevelAgreements ServiceLevelAgreement { get; set; }
    }
}
