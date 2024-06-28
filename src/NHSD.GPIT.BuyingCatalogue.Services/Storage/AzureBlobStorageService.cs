using System;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Microsoft.Extensions.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;

namespace NHSD.GPIT.BuyingCatalogue.Services.Storage;

public class AzureBlobStorageService : IAzureBlobStorageService
{
    private readonly IAzureBlobContainerClientFactory clientFactory;
    private readonly ILogger<AzureBlobStorageService> logger;

    public AzureBlobStorageService(
        IAzureBlobContainerClientFactory clientFactory,
        ILogger<AzureBlobStorageService> logger)
    {
        this.clientFactory = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<MemoryStream> DownloadAsync(BlobDocument blobDocument)
    {
        try
        {
            var client = clientFactory.GetBlobContainerClient(blobDocument.ContainerName);

            var blobResponse = await client.GetBlobClient(blobDocument.DocumentName).DownloadAsync();

            var memoryStream = new MemoryStream();

            await blobResponse.Value.Content.CopyToAsync(memoryStream);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }
        catch (RequestFailedException ex)
        {
            logger.LogError(ex, "Blob file requested {BlobFile} but returned error", blobDocument.DocumentName);

            return null;
        }
    }

    public async Task UploadAsync(BlobDocument blobDocument, Stream contents)
    {
        var client = clientFactory.GetBlobContainerClient(blobDocument.ContainerName);
        var blobClient = client.GetBlobClient(blobDocument.DocumentName);
        if (await blobClient.ExistsAsync() == true)
        {
            logger.LogError("Attempt to add a duplicate blob object {BlobFile}", blobDocument.DocumentName);

            return;
        }

        await blobClient.UploadAsync(contents);
    }
}
