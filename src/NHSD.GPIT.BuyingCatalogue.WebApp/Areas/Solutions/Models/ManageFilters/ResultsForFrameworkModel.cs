using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;
using NHSD.GPIT.BuyingCatalogue.WebApp.Models;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Areas.Solutions.Models.ManageFilters
{
    public class ResultsForFrameworkModel : NavBaseModel
    {
        public ResultsForFrameworkModel()
        {
        }

        public ResultsForFrameworkModel(
            string internalOrgId,
            int filterId,
            FrameworkFilterInfo framework,
            List<CatalogueItem> catalogueItems)
        {
            InternalOrgId = internalOrgId;
            FilterId = filterId;
            Framework = framework;
            CatalogueItems = catalogueItems.Where(x => x.Solution.CatalogueItem.Solution.FrameworkSolutions.Any(x => x.FrameworkId == Framework.Id)).ToList();
        }

        public string InternalOrgId { get; set; }

        public int FilterId { get; set; }

        public FrameworkFilterInfo Framework { get; set; }

        public List<CatalogueItem> CatalogueItems { get; init; }
    }
}
