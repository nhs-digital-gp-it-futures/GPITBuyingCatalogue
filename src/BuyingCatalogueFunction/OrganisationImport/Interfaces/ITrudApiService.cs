using System.Threading.Tasks;
using BuyingCatalogueFunction.OrganisationImport.Models;

namespace BuyingCatalogueFunction.OrganisationImport.Interfaces;

public interface ITrudApiService
{
    Task<TrudApiResponse.Release> GetLatestReleaseInfo();
}
