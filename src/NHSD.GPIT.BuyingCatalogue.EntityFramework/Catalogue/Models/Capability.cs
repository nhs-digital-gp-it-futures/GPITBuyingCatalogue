using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    [Serializable]
    public sealed class Capability : IAudited
    {
        public Capability()
        {
            CatalogueItemCapabilities = new HashSet<CatalogueItemCapability>();
            Epics = new HashSet<Epic>();
            CapabilityEpics = new HashSet<CapabilityEpic>();
            StandardCapabilities = new HashSet<StandardCapability>();
            FrameworkCapabilities = new HashSet<FrameworkCapability>();
        }

        public int Id { get; set; }

        public string CapabilityRef { get; set; }

        public string Name { get; set; }

        public string NameWithStatusSuffix => Status == CapabilityStatus.Expired
            ? $"{Name} (expired)"
            : Name;

        public string Description { get; set; }

        public string SourceUrl { get; set; }

        public DateTime EffectiveDate { get; set; }

        public int CategoryId { get; set; }

        public DateTime LastUpdated { get; set; }

        public int? LastUpdatedBy { get; set; }

        public AspNetUser LastUpdatedByUser { get; set; }

        public CapabilityCategory Category { get; set; }

        public ICollection<FrameworkCapability> FrameworkCapabilities { get; set; }

        public ICollection<Epic> Epics { get; set; }

        public ICollection<CapabilityEpic> CapabilityEpics { get; set; }

        public ICollection<CatalogueItemCapability> CatalogueItemCapabilities { get; set; }

        public ICollection<StandardCapability> StandardCapabilities { get; set; }

        public CapabilityStatus Status { get; set; }

        public IReadOnlyCollection<Epic> GetAllMustEpics() => CapabilityEpics
            .Where(c => c.CompliancyLevel == CompliancyLevel.Must)
            .Select(c => c.Epic)
            .ToList();

        public IReadOnlyCollection<Epic> GetAllMayEpics() => CapabilityEpics
            .Where(c => c.CompliancyLevel == CompliancyLevel.May)
            .Select(c => c.Epic)
            .ToList();
    }
}
