using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Csv;

public interface IServiceRecipientImportService
{
    Task CreateServiceRecipientTemplate(MemoryStream stream, IEnumerable<ServiceRecipientImportModel> serviceRecipients = null);

    Task<IList<ServiceRecipientImportModel>> ReadFromStream(Stream stream);

    Task Store(DistributedCacheKey cacheKey, IList<ServiceRecipientImportModel> importedServiceRecipients);

    Task Clear(DistributedCacheKey cacheKey);

    Task<IList<ServiceRecipientImportModel>> GetCached(DistributedCacheKey cacheKey);
}
