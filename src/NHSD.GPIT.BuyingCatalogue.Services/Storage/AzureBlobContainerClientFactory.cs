using Azure.Storage.Blobs;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;

namespace NHSD.GPIT.BuyingCatalogue.Services.Storage;

public class AzureBlobContainerClientFactory : IAzureBlobContainerClientFactory
{
    private readonly BlobServiceClient blobClient;

    public AzureBlobContainerClientFactory(
        AzureBlobSettings settings)
    {
        blobClient = new(settings.ConnectionString);
    }

    public BlobContainerClient GetBlobContainerClient(string containerName) =>
        blobClient.GetBlobContainerClient(containerName);
}
