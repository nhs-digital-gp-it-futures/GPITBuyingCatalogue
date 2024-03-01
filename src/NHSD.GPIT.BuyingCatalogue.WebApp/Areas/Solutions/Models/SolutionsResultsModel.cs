using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.Filters;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models
{
    public class SolutionsResultsModel
    {
        public PageOptions PageOptions { get; set; }

        public bool FilterResultView { get; set; }

        public RequestedFilters Filters { get; set; }

        public IList<CatalogueItem> CatalogueItems { get; init; }
    }
}
