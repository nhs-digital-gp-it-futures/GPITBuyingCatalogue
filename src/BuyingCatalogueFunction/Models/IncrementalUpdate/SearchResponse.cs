using System.Collections.Generic;
using System.Linq;
using BuyingCatalogueFunction.Models.Ods;

namespace BuyingCatalogueFunction.Models.IncrementalUpdate
{
    public class SearchResponse
    {
        public List<OrgSummary> Organisations { get; set; } = new();

        public List<string> OrganisationIds => Organisations?
            .Select(x => x.OrgLink.Split("/").Last())
            .Distinct()
            .ToList() ?? Enumerable.Empty<string>().ToList();
    }
}
