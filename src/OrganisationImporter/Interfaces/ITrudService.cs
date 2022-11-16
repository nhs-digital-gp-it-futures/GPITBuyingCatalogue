using OrganisationImporter.Models;

namespace OrganisationImporter.Interfaces;

public interface ITrudService
{
    Task<OrgRefData> GetTrudData(Uri url);

    Task SaveTrudDataAsync(OdsOrganisationMapping mappedData);
}
