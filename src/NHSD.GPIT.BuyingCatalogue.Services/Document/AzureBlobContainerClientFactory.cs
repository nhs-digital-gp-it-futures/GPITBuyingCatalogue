using Azure.Storage.Blobs;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;

namespace NHSD.GPIT.BuyingCatalogue.Services.Document
{
    public static class AzureBlobContainerClientFactory
    {
        public static BlobContainerClient Create(AzureBlobStorageSettings settings)
        {
            var retrySettings = settings.Retry;

            var options = retrySettings == null
                ? new BlobClientOptions()
                : new BlobClientOptions
                {
                    Retry =
                    {
                        Mode = retrySettings.Mode,
                        MaxRetries = retrySettings.MaxRetries,
                        Delay = retrySettings.Delay,
                        MaxDelay = retrySettings.MaxDelay,
                    },
                };

            return new BlobServiceClient(settings.ConnectionString, options)
                .GetBlobContainerClient(settings.ContainerName);
        }
    }
}
