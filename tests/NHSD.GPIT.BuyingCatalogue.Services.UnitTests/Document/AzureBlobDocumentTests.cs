using System.IO;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Services.Document;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Document
{
    public static class AzureBlobDocumentTests
    {
        [Fact]
        public static void ContentInfo_ReturnsExpectedValue()
        {
            using var stream = new MemoryStream();

            var downloadInfo = BlobsModelFactory.BlobDownloadInfo(content: stream);

            var document = new AzureBlobDocument(downloadInfo);

            document.Content.Should().BeSameAs(downloadInfo.Content);
        }

        [Fact]
        public static void ContentType_ReturnsExpectedValue()
        {
            const string contentType = "test/content";

            var downloadInfo = BlobsModelFactory.BlobDownloadInfo(contentType: contentType);

            var document = new AzureBlobDocument(downloadInfo);

            document.ContentType.Should().Be(downloadInfo.ContentType);
        }
    }
}
