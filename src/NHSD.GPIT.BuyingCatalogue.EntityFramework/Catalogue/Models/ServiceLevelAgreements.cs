using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public class ServiceLevelAgreements
    {
        public ServiceLevelAgreements()
        {
            Contacts = new HashSet<SlaContact>();
            ServiceHours = new HashSet<SlaServiceHours>();
            ServiceLevels = new HashSet<SlaServiceLevels>();
        }

        public CatalogueItemId SolutionId { get; set; }

        public SlaType SlaType { get; set; }

        public ICollection<SlaContact> Contacts { get; set; }

        public ICollection<SlaServiceHours> ServiceHours { get; set; }

        public ICollection<SlaServiceLevels> ServiceLevels { get; set; }

        public virtual Solution Solution { get; set; }
    }
}
