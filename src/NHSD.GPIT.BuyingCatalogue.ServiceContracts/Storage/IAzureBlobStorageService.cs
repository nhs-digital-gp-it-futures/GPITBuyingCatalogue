using System.IO;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;

public interface IAzureBlobStorageService
{
    Task<MemoryStream> DownloadAsync(BlobDocument blobDocument);

    Task UploadAsync(BlobDocument blobDocument, Stream contents);
}
