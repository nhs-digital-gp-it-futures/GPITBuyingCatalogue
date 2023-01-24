using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public class ServiceLevelAgreements : IAudited
    {
        public ServiceLevelAgreements()
        {
            Contacts = new HashSet<SlaContact>();
            ServiceHours = new HashSet<ServiceAvailabilityTimes>();
            ServiceLevels = new HashSet<SlaServiceLevel>();
        }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public CatalogueItemId SolutionId { get; set; }

        public SlaType SlaType { get; set; }

        public ICollection<SlaContact> Contacts { get; set; }

        public ICollection<ServiceAvailabilityTimes> ServiceHours { get; set; }

        public ICollection<SlaServiceLevel> ServiceLevels { get; set; }

        public virtual Solution Solution { get; set; }
    }
}
