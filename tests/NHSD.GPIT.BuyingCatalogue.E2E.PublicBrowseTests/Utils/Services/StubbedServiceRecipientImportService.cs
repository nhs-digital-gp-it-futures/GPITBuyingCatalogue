using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.Services;

public class StubbedServiceRecipientImportService : IServiceRecipientImportService
{
    private readonly IList<ServiceRecipientImportModel> importedServiceRecipients =
        new List<ServiceRecipientImportModel>
        {
            new() { OdsCode = "ABC123", Organisation = "Test Org 1" },
            new() { OdsCode = "Y03508", Organisation = "ASSURA ER LLP HULL DERMATOLOGY" },
            new() { OdsCode = "Y07021", Organisation = "BEVAN LTD" },
        };

    public Task CreateServiceRecipientTemplate(MemoryStream stream)
    {
        throw new System.NotImplementedException();
    }

    public Task<IList<ServiceRecipientImportModel>> ReadFromStream(Stream stream)
        => Task.FromResult(importedServiceRecipients);

    public IList<ServiceRecipientImportModel> GetCached(ServiceRecipientCacheKey cacheKey)
        => importedServiceRecipients;

    public void Store(ServiceRecipientCacheKey cacheKey, IList<ServiceRecipientImportModel> importedServiceRecipients)
    {
    }

    public void Clear(ServiceRecipientCacheKey cacheKey)
    {
    }
}
