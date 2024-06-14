using System.Collections.Generic;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

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
            string frameworkId,
            string frameworkName,
            bool isExpired,
            List<CatalogueItem> catalogueItems,
            bool fromFilter)
        {
            InternalOrgId = internalOrgId;
            FilterId = filterId;
            FrameworkId = frameworkId;
            FrameworkName = frameworkName;
            IsExpired = isExpired;
            CatalogueItems = catalogueItems;
            FromFilter = fromFilter;
        }

        public string InternalOrgId { get; set; }

        public int FilterId { get; set; }

        public string FrameworkId { get; set; }

        public string FrameworkName { get; set; }

        public bool IsExpired { get; set; }

        public List<CatalogueItem> CatalogueItems { get; init; }

        public bool FromFilter { get; set; }
    }
}
