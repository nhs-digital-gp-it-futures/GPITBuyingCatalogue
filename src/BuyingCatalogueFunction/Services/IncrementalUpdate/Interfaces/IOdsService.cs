using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Models.Ods;

namespace BuyingCatalogueFunction.Services.IncrementalUpdate.Interfaces
{
    public interface IOdsService
    {
        Task<Org> GetOrganisation(string orgId);

        Task<IEnumerable<Relationship>> GetRelationships();

        Task<IEnumerable<Role>> GetRoles();

        Task<IEnumerable<string>> SearchByLastChangeDate(DateTime lastChangedDate);
    }
}
