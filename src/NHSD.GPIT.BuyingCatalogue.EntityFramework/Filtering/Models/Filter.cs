using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models
{
    [Serializable]
    public partial class Filter : IAudited
    {
        public Filter()
        {
            FilterCapabilities = new HashSet<FilterCapability>();
            FilterEpics = new HashSet<FilterEpic>();
            FilterHostingTypes = new HashSet<FilterHostingType>();
            FilterClientApplicationTypes = new HashSet<FilterClientApplicationType>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int OrganisationId { get; set; }

        public string FrameworkId { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public DateTime? LastPublished { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public virtual Organisation Organisation { get; set; }

        public virtual Framework Framework { get; set; }

        public ICollection<FilterCapability> FilterCapabilities { get; set; }

        public ICollection<FilterEpic> FilterEpics { get; set; }

        public ICollection<FilterHostingType> FilterHostingTypes { get; set; }

        public ICollection<FilterClientApplicationType> FilterClientApplicationTypes { get; set; }
    }
}
