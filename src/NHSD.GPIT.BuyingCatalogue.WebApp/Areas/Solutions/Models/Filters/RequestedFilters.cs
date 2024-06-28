using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    public record RequestedFilters(
            string Selected,
            string Search,
            string SelectedFrameworkId,
            string SelectedApplicationTypeIds,
            string SelectedHostingTypeIds,
            string SelectedIntegrations,
            string SortBy)
    {
        public bool SearchCriteriaApplied => GetFiltersArray().Any(f => !string.IsNullOrWhiteSpace(f));

        public int FilterCount => GetFiltersArray().Count(f => !string.IsNullOrWhiteSpace(f));

        public Dictionary<int, string[]> GetCapabilityAndEpicIds() => SolutionsFilterHelper.ParseCapabilityAndEpicIds(Selected);

        public Dictionary<SupportedIntegrations, int[]> GetIntegrationsAndTypes() =>
            SolutionsFilterHelper.ParseIntegrationAndTypeIds(SelectedIntegrations);

        public object ToRouteValues(int? page = null) => new
        {
            selected = Selected,
            search = Search,
            selectedFrameworkId = SelectedFrameworkId,
            selectedApplicationTypeIds = SelectedApplicationTypeIds,
            selectedHostingTypeIds = SelectedHostingTypeIds,
            selectedIntegrations = SelectedIntegrations,
            sortBy = SortBy,
            page = page ?? 1,
        };

        private string[] GetFiltersArray() =>
            [
                Selected,
                SelectedFrameworkId,
                SelectedApplicationTypeIds,
                SelectedHostingTypeIds,
                SelectedIntegrations];
    }
}
