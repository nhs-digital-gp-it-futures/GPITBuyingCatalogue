using System.Collections.Generic;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Models.Ods;

namespace BuyingCatalogueFunction.Services.IncrementalUpdate.Interfaces
{
    public interface IOdsOrganisationService
    {
        Task AddRelationshipTypes(List<Relationship> relationships);

        Task AddRoleTypes(List<Role> roles);

        Task AddOrganisationRelationships(List<Org> organisations);

        Task AddOrganisationRoles(List<Org> organisations);

        Task UpsertOrganisations(List<Org> organisations);
    }
}
