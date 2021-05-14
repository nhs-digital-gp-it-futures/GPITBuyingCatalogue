using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Blobs;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;

namespace NHSD.GPIT.BuyingCatalogue.Services.Document
{
    public sealed class AzureBlobDocumentRepository : IAzureBlobDocumentRepository
    {
        private readonly BlobContainerClient _client;
        private readonly AzureBlobStorageSettings _blobStorageSettings;

        public AzureBlobDocumentRepository(BlobContainerClient client, AzureBlobStorageSettings blobStorageSettings)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _blobStorageSettings = blobStorageSettings ?? throw new ArgumentNullException(nameof(blobStorageSettings));
        }

        public Task<IDocument> DownloadAsync(string documentName)
        {
            return DownloadAsync(_blobStorageSettings.DocumentDirectory, documentName);
        }

        public async Task<IDocument> DownloadAsync(string? directoryName, string documentName)
        {
            try
            {
                var blobName = directoryName + "/" + documentName;
                var downloadInfo = await _client.GetBlobClient(blobName).DownloadAsync();

                return new AzureBlobDocument(downloadInfo);
            }
            catch (RequestFailedException e)
            {
                throw new DocumentRepositoryException(e, e.Status);
            }
        }

        public async IAsyncEnumerable<string> GetFileNamesAsync(string directory)
        {
            var all = _client.GetBlobsAsync(prefix: $"{directory}/");

            await foreach (var blob in all)
            {
                yield return Path.GetFileName(blob.Name);
            }
        }
    }
}
