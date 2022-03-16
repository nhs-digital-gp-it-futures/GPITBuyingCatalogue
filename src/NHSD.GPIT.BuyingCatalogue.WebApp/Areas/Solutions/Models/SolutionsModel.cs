using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    [ExcludeFromCodeCoverage]
    public class SolutionsModel
    {
        public SolutionsModel()
        {
        }

        public SolutionsModel(List<KeyValuePair<EntityFramework.Catalogue.Models.Framework, int>> frameworks)
        {
            FrameworkFilters = frameworks
               .Select(f =>
               new FrameworkFilter
               {
                   FrameworkId = f.Key.Id,
                   FrameworkFullName = $"{f.Key.ShortName} {(f.Key.Id == "All" ? "frameworks" : "framework")}",
                   Count = f.Value,
               }).ToList();
        }

        public IList<CatalogueItem> CatalogueItems { get; init; }

        public IList<FrameworkFilter> FrameworkFilters { get; init; } = new List<FrameworkFilter>();

        public IList<CapabilityCategoryFilter> CategoryFilters { get; init; } = new List<CapabilityCategoryFilter>();

        public IList<CapabilitiesFilter> FoundationCapabilities { get; init; } = new List<CapabilitiesFilter>();

        public int CountOfSolutionsWithFoundationCapability { get; init; }

        public string FoundationCapabilitiesCapabilityRef => "FC";

        public PageOptions Options { get; init; }

        public string SelectedFramework { get; init; }

        public string SelectedCapabilities { get; init; }

        public CatalogueFilterSearchSummary SearchSummary { get; set; }
    }
}
