using System;
using System.Threading.Tasks;
using BuyingCatalogueFunction.OrganisationImport.Models;

namespace BuyingCatalogueFunction.OrganisationImport.Interfaces;

public interface ITrudService
{
    Task<bool> HasImportedLatestRelease(DateTime latestReleaseDate);

    Task<OrgRefData> GetTrudDataAsync(Uri url);

    Task SaveTrudDataAsync(OdsOrganisationMapping mappedData);
}
