using System;
using Azure.Storage.Blobs;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;

namespace NHSD.GPIT.BuyingCatalogue.Services.Storage;

public class AzureBlobContainerClientFactory : IAzureBlobContainerClientFactory
{
    private readonly BlobServiceClient blobClient;

    public AzureBlobContainerClientFactory(
        BlobServiceClient blobClient)
    {
        this.blobClient = blobClient ?? throw new ArgumentNullException(nameof(blobClient));
    }

    public BlobContainerClient GetBlobContainerClient(string containerName) =>
        blobClient.GetBlobContainerClient(containerName);
}
