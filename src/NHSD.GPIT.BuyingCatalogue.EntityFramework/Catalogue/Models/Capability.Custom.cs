using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models
{
    public sealed partial class Capability
    {
        public IReadOnlyCollection<Epic> GetActiveMustEpics() => CapabilityEpics
            .Where(c => c.Epic.IsActive && c.CompliancyLevel == CompliancyLevel.Must)
            .Select(c => c.Epic)
            .ToList();

        public IReadOnlyCollection<Epic> GetActiveMayEpics() => CapabilityEpics
            .Where(c => c.Epic.IsActive && c.CompliancyLevel == CompliancyLevel.May)
            .Select(c => c.Epic)
            .ToList();
    }
}
