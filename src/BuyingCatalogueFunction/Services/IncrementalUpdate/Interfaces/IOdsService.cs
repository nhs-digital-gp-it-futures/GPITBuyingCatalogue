using System;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Models.IncrementalUpdate;
using BuyingCatalogueFunction.Models.Ods;

namespace BuyingCatalogueFunction.Services.IncrementalUpdate.Interfaces
{
    public interface IOdsService
    {
        Task<SearchResult> SearchByLastChangedDate(DateTime lastChangedDate);

        Task<Organisation> GetOrganisation(string orgId);
    }
}
