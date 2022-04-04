using System.Collections.Generic;
using System.Linq;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class CatalogueFilterSearchSummary
    {
        public CatalogueFilterSearchSummary()
        {
        }

        public CatalogueFilterSearchSummary(
            Dictionary<string, int> capabilitiesAndEpics,
            string frameworkName,
            string searchTerm)
        {
            CapabilitiesAndEpics = capabilitiesAndEpics
                .OrderByDescending(o => o.Value)
                .ThenBy(o => o.Key)
                .ToDictionary(o => o.Key, o => o.Value);

            FrameworkName = frameworkName;
            SearchTerm = searchTerm;
        }

        public Dictionary<string, int> CapabilitiesAndEpics { get; set; }

        public string FrameworkName { get; set; }

        public string SearchTerm { get; set; }
    }
}
