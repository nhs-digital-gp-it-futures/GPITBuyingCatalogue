using System.Collections.Generic;
using BuyingCatalogueFunction.Models.Ods;

namespace BuyingCatalogueFunction.Models.IncrementalUpdate
{
    public class SearchResult
    {
        public List<OrganisationSummary> Organisations { get; set; } = new();
    }
}
