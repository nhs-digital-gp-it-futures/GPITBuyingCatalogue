using System.Collections.Generic;
using System.Linq;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.FilterModels;

namespace NHSD.GPIT.BuyingCatalogue.WebApp.Models.Shared.Shorlists
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
            List<CatalogueItem> catalogueItems,
            bool fromFilter)
        {
            InternalOrgId = internalOrgId;
            FilterId = filterId;
            Framework = framework;
            CatalogueItems = catalogueItems;
            FromFilter = fromFilter;
        }

        public string InternalOrgId { get; set; }

        public int FilterId { get; set; }

        public FrameworkFilterInfo Framework { get; set; }

        public List<CatalogueItem> CatalogueItems { get; init; }

        public bool FromFilter { get; set; }
    }
}
