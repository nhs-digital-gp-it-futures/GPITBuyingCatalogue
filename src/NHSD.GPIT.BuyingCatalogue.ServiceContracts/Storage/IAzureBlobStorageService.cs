using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;

public interface IAzureBlobStorageService
{
    Task<BlobDownloadInfo> DownloadAsync(BlobDocument blobDocument);

    Task UploadAsync(BlobDocument blobDocument, Stream contents);
}
