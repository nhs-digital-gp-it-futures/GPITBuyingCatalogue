using OrganisationImporter.Models;

namespace OrganisationImporter.Interfaces;

public interface ITrudService
{
    Task<OrgRefData> GetTrudDataAsync(Uri url);

    Task SaveTrudDataAsync(OdsOrganisationMapping mappedData);
}
