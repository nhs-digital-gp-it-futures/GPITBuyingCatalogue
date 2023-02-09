using Azure.Storage.Blobs;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;

public interface IAzureBlobContainerClientFactory
{
    BlobContainerClient GetBlobContainerClient(string containerName);
}
