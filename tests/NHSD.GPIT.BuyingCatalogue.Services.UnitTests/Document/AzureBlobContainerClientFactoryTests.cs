using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.Services.Document;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Document
{
    public static class AzureBlobContainerClientFactoryTests
    {
        [Fact]
        public static void Create_NullRetryOptions_ReturnsContainerClient()
        {
            const string accountName = "devstoreaccount1";
            const string containerName = "Container";
            const string uri = "http://127.0.0.1:10000/" + accountName;
            const string connectionString =
                "DefaultEndpointsProtocol=http;AccountName="
                + accountName + "UnitTest;AccountKey=;BlobEndpoint="
                + uri;

            var settings = new AzureBlobStorageSettings
            {
                ConnectionString = connectionString,
                ContainerName = containerName,
            };

            var client = AzureBlobContainerClientFactory.Create(settings);

            client.Should().NotBeNull();
            client.AccountName.Should().Be(accountName);
            client.Name.Should().Be(containerName);
        }

        [Fact]
        public static void Create_WithRetryOptions_ReturnsContainerClient()
        {
            const string accountName = "devstoreaccount1";
            const string containerName = "Container";
            const string uri = "http://127.0.0.1:10000/" + accountName;
            const string connectionString =
                "DefaultEndpointsProtocol=http;AccountName="
                + accountName + "UnitTest;AccountKey=;BlobEndpoint="
                + uri;

            var settings = new AzureBlobStorageSettings
            {
                ConnectionString = connectionString,
                ContainerName = containerName,
                Retry = new AzureBlobStorageRetrySettings(),
            };

            var client = AzureBlobContainerClientFactory.Create(settings);

            client.Should().NotBeNull();
            client.AccountName.Should().Be(accountName);
            client.Name.Should().Be(containerName);
        }
    }
}
