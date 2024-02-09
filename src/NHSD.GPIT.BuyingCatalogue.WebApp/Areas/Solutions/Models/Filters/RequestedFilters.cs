using System;
using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.Services.ServiceHelpers;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters
{
    public record RequestedFilters(
            string Selected,
            string Search,
            string SelectedFrameworkId,
            string SelectedApplicationTypeIds,
            string SelectedHostingTypeIds,
            string SelectedIM1Integrations,
            string SelectedGPConnectIntegrations,
            string SelectedInteroperabilityOptions,
            string SortBy)
    {
        public bool SearchCriteriaApplied => GetFiltersArray().Any(f => !string.IsNullOrWhiteSpace(f));

        public int FilterCount => GetFiltersArray().Count(f => !string.IsNullOrWhiteSpace(f));

        private string[] GetFiltersArray() =>
            [
                Selected,
                Search,
                SelectedFrameworkId,
                SelectedApplicationTypeIds,
                SelectedHostingTypeIds,
                SelectedIM1Integrations,
                SelectedGPConnectIntegrations,
                SelectedInteroperabilityOptions];

        public Dictionary<int, string[]> GetCapabilityAndEpicIds() => SolutionsFilterHelper.ParseCapabilityAndEpicIds(Selected);

        public object ToRouteValues() => new
        {
            selected = Selected,
            search = Search,
            selectedFrameworkId = SelectedFrameworkId,
            selectedApplicationTypeIds = SelectedApplicationTypeIds,
            selectedHostingTypeIds = SelectedHostingTypeIds,
            selectedIM1Integrations = SelectedIM1Integrations,
            selectedGPConnectIntegrations = SelectedGPConnectIntegrations,
            selectedInteroperabilityOptions = SelectedInteroperabilityOptions,
            sortBy = SortBy,
        };
    }
}
