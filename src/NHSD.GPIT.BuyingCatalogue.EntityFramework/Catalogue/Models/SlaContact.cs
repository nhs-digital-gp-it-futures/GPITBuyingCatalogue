using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public class SlaContact
    {
        public int Id { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public string Channel { get; set; }

        public string ContactInformation { get; set; }

        public DateTime TimeFrom { get; set; }

        public DateTime TimeUntil { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public ServiceLevelAgreements ServiceLevelAgreement { get; set; }
    }
}
