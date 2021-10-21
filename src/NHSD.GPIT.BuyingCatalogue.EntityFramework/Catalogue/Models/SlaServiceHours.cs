using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public class SlaServiceHours
    {
        public int Id { get; set; }

        public string Category { get; set; }

        public TimeSpan TimeFrom { get; set; }

        public TimeSpan TimeUntil { get; set; }

        public string ApplicableDays { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public ServiceLevelAgreements ServiceLevelAgreement { get; set; }
    }
}
