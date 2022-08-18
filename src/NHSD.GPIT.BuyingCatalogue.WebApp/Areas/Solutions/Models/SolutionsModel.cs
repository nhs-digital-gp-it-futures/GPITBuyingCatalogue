using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using static NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.PageOptions;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    [ExcludeFromCodeCoverage]
    public class SolutionsModel
    {
        public SolutionsModel()
        {
        }

        public IList<CatalogueItem> CatalogueItems { get; init; }

        public CatalogueFilterSearchSummary SearchSummary { get; init; }

        // only used for the UI component
        public SortOptions SelectedSortOption { get; init; }

        public PageOptions PageOptions { get; set; }

        public IEnumerable<SelectListItem> SortOptions =>
            Enum.GetValues(typeof(SortOptions))
                .Cast<SortOptions>()
                .Where(e => !e.Equals(PageOptions.SortOptions.None))
                .Select(e => new SelectListItem(e.Name(), e.ToString(), e == SelectedSortOption));

        public bool SearchCriteriaApplied =>
            !string.IsNullOrWhiteSpace(SearchSummary?.SelectedCapabilityIds)
            || !string.IsNullOrWhiteSpace(SearchSummary?.SearchTerm);
    }
}
