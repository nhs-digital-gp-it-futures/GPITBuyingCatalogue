using System.Collections.Generic;
using BuyingCatalogueFunction.IncrementalUpdate.Models.Ods;

namespace BuyingCatalogueFunction.IncrementalUpdate.Models
{
    public class IncrementalUpdateData
    {
        public List<Org> Organisations { get; set; } = new();

        public List<Org> RelatedOrganisations { get; set; } = new();

        public List<Relationship> Relationships { get; set; } = new();

        public List<Role> Roles { get; set; } = new();
    }
}
