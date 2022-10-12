using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;

public interface IServiceRecipientImportService
{
    Task CreateServiceRecipientTemplate(MemoryStream stream);

    Task<IList<ServiceRecipientImportModel>> ReadFromStream(Stream stream);

    void StoreOrUpdate(ServiceRecipientCacheKey cacheKey, IList<ServiceRecipientImportModel> importedServiceRecipients);

    void Clear(ServiceRecipientCacheKey cacheKey);

    IList<ServiceRecipientImportModel> GetCached(ServiceRecipientCacheKey cacheKey);
}
