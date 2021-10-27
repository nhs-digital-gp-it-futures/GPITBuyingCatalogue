using System;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public class SlaContact
    {
        public int Id { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public string Channel { get; set; }

        public string ContactInformation { get; set; }

        public TimeSpan TimeFrom { get; set; }

        public TimeSpan TimeUntil { get; set; }

        public ServiceLevelAgreements ServiceLevelAgreement { get; set; }
    }
}
