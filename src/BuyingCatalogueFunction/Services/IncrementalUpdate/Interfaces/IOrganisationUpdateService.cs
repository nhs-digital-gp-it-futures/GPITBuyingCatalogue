using System.Collections.Generic;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Models.Ods;

namespace BuyingCatalogueFunction.Services.IncrementalUpdate.Interfaces
{
    public interface IOrganisationUpdateService
    {
        Task Upsert(Organisation organisation);

        Task AddMissingOrganisations(IEnumerable<string> organisationIds);

        Task AddRelationships(Organisation organisation);

        Task AddRoles(Organisation organisation);

        Task AddRelationshipTypes(IEnumerable<string> relationshipIds);

        Task AddRoleTypes(IEnumerable<string> roleIds);
    }
}
