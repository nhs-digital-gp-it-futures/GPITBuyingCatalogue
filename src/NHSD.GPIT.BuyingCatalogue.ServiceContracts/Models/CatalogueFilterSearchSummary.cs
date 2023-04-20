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
            string selectedCapabilityids,
            string selectedEpicIds)
        {
            CapabilitiesAndEpics = capabilitiesAndEpics
                .OrderByDescending(o => o.CountOfEpics)
                .ThenBy(o => o.CapabilityName)
                .ToList();
            SearchTerm = searchTerm;

            SelectedCapabilityIds = selectedCapabilityids;

            SelectedEpicIds = selectedEpicIds;
        }

        public ICollection<CapabilitiesAndCountModel> CapabilitiesAndEpics { get; set; }

        public string SearchTerm { get; set; }

        public string SelectedCapabilityIds { get; init; }

        public string SelectedEpicIds { get; init; }

        public string SelectedFrameworkIds { get; init; }
    }
}
