using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SolutionsFilterModels;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models
{
    public sealed class CatalogueFilterSearchSummary
    {
        public CatalogueFilterSearchSummary()
        {
        }

        public CatalogueFilterSearchSummary(
            List<CapabilitiesAndCountModel> capabilitiesAndEpics,
            string searchTerm,
            string selected,
            bool includesEpics)
        {
            CapabilitiesAndEpics = capabilitiesAndEpics
                .OrderByDescending(o => o.CountOfEpics)
                .ThenBy(o => o.CapabilityName)
                .ToList();
            SearchTerm = searchTerm;

            Selected = selected;

            IncludesEpics = includesEpics;
        }

        public ICollection<CapabilitiesAndCountModel> CapabilitiesAndEpics { get; set; }

        public string SearchTerm { get; set; }

        public bool IncludesEpics { get; set; }

        public string Selected { get; init; }
    }
}
