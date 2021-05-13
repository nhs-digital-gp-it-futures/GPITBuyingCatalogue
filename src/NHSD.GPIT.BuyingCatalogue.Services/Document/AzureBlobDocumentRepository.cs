using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;

namespace NHSD.GPIT.BuyingCatalogue.Services.Document
{
    public sealed class AzureBlobDocumentRepository : IDocumentRepository
    {
        private readonly BlobContainerClient client;
        private readonly IAzureBlobStorageSettings blobStorageSettings;

        public AzureBlobDocumentRepository(BlobContainerClient client, IAzureBlobStorageSettings blobStorageSettings)
        {
            this.client = client;
            this.blobStorageSettings = blobStorageSettings;
        }

        public Task<IDocument> DownloadAsync(string documentName)
        {
            return DownloadAsync(blobStorageSettings.DocumentDirectory, documentName);
        }

        public async Task<IDocument> DownloadAsync(string? directoryName, string documentName)
        {
            try
            {
                var blobName = directoryName + "/" + documentName;
                var downloadInfo = await client.GetBlobClient(blobName).DownloadAsync();

                return new AzureBlobDocument(downloadInfo);
            }
            catch (RequestFailedException e)
            {
                throw new DocumentRepositoryException(e, e.Status);
            }
        }

        public async IAsyncEnumerable<string> GetFileNamesAsync(string directory)
        {
            var all = client.GetBlobsAsync(prefix: $"{directory}/");

            await foreach (var blob in all)
            {
                yield return Path.GetFileName(blob.Name);
            }
        }
    }
}
