using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.SolutionsFilterModels;
using NHSD.GPIT.BuyingCatalogue.UI.Components.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Shorlists;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models
{
    [ExcludeFromCodeCoverage]
    public class SolutionsFilterResultsModel
    {
        public string DateAndTime => $"{DateTime.Now:d MMMM yyyy HH:mm}";

        public PageTitleModel Title { get; init; }

        public IList<CatalogueItem> CatalogueItems { get; init; }

        public int ResultsCount { get; init; }

        public List<ResultsForFrameworkModel> ResultsForFramework { get; init; }

        public ReviewFilterModel ReviewFilter { get; init; }

        public ICollection<CapabilitiesAndCountModel> CapabilitiesAndEpics { get; init; }
    }
}
