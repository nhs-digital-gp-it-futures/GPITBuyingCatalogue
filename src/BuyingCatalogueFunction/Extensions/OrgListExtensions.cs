using System.Collections.Generic;
using System.Linq;
using BuyingCatalogueFunction.Models.Ods;

namespace BuyingCatalogueFunction.Extensions
{
    public static class OrgListExtensions
    {
        public static IEnumerable<string> RelatedOrganisationIds(this List<Org> organisations)
        {
            return organisations?
                .SelectMany(x => x.Rels?.Rel ?? Enumerable.Empty<OrgRel>())
                .Select(x => x.Target.OrgId.extension)
                .Distinct()
                .ToList() ?? Enumerable.Empty<string>();
        }
    }
}
