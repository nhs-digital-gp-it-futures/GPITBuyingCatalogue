using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Settings
{
    public static class AzureBlobStorageSettingsTests
    {
        [Fact]
        public static void Uri_HasConnectionString_ReturnsUri()
        {
            // ReSharper disable once StringLiteralTypo
            const string uri = "http://127.0.0.1:10000/devstoreaccount1";

            const string connectionString =
                "DefaultEndpointsProtocol=http;AccountName=UnitTest;AccountKey=;BlobEndpoint=" + uri;

            var settings = new AzureBlobStorageSettings { ConnectionString = connectionString };

            Assert.NotNull(settings.Uri);
            settings.Uri.AbsoluteUri.Should().BeEquivalentTo(uri);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("DefaultEndpointsProtocol=http;NotValid=foo;")]
        public static void Uri_NullConnectionString_ReturnsNull(string connectionString)
        {
            var settings = new AzureBlobStorageSettings { ConnectionString = connectionString };

            settings.Uri.Should().BeNull();
        }
    }
}
