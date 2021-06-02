using System.Text.Json;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.Framework.UnitTests.Settings
{
    [TestFixture]
    internal static class AzureBlobStorageSettingsTests
    {
        [Test]
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

        [TestCase(null)]
        [TestCase("DefaultEndpointsProtocol=http;NotValid=foo;")]
        public static void Uri_NullConnectionString_ReturnsNull(string connectionString)
        {
            var settings = new AzureBlobStorageSettings { ConnectionString = connectionString };

            settings.Uri.Should().BeNull();
        }      
    }
}
