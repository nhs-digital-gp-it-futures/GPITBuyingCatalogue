using System;
using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class Standard : IAudited
    {
        public Standard()
        {
            StandardCapabilities = new HashSet<StandardCapability>();
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public StandardType StandardType { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<StandardCapability> StandardCapabilities { get; set; }
    }
}
