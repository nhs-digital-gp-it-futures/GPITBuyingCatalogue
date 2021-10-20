using System;
using System.Collections.Generic;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed class Capability
    {
        public Capability()
        {
            CatalogueItemCapabilities = new HashSet<CatalogueItemCapability>();
            Epics = new HashSet<Epic>();
        }

        public int Id { get; set; }

        public string CapabilityRef { get; set; }

        public string Version { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string SourceUrl { get; set; }

        public DateTime EffectiveDate { get; set; }

        public int CategoryId { get; set; }

        public CapabilityCategory Category { get; set; }

        public ICollection<FrameworkCapability> FrameworkCapabilities { get; set; }

        public ICollection<Epic> Epics { get; set; }

        public ICollection<CatalogueItemCapability> CatalogueItemCapabilities { get; set; }

        public CapabilityStatus Status { get; set; }
    }
}
