using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models
{
    public sealed class Filter : IAudited
    {
        public Filter()
        {
            Capabilities = new HashSet<Capability>();
            Epics = new HashSet<Epic>();
            FilterHostingTypes = new HashSet<FilterHostingType>();
            FilterApplicationTypes = new HashSet<FilterClientApplicationType>();
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public int OrganisationId { get; set; }

        public string FrameworkId { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public bool IsDeleted { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public Organisation Organisation { get; set; }

        public Framework Framework { get; set; }

        public ICollection<Capability> Capabilities { get; set; }

        public ICollection<Epic> Epics { get; set; }

        public ICollection<FilterHostingType> FilterHostingTypes { get; set; }

        public ICollection<FilterClientApplicationType> FilterApplicationTypes { get; set; }
    }
}
