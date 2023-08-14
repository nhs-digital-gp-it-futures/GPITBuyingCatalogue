using System.Collections.Generic;
using System.Linq;
using BuyingCatalogueFunction.IncrementalUpdate.Models.Ods;

namespace BuyingCatalogueFunction.IncrementalUpdate.Models
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
