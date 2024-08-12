using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public class ServiceAvailabilityTimes : IAudited
    {
        public int Id { get; set; }

        [StringLength(100)]
        public string Category { get; set; }

        public DateTime TimeFrom { get; set; }

        public DateTime TimeUntil { get; set; }

        [Obsolete("Replaced with IncludedDays")]
        public string ApplicableDays { get; set; }

        public ICollection<Iso8601DayOfWeek> IncludedDays { get; set; } = new HashSet<Iso8601DayOfWeek>();

        public bool IncludesBankHolidays { get; set; }

        [StringLength(500)]
        public string AdditionalInformation { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public ServiceLevelAgreements ServiceLevelAgreement { get; set; }
    }
}
