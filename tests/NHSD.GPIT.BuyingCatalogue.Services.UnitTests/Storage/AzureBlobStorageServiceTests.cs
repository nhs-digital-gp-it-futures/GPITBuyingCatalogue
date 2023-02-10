using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;
using NHSD.GPIT.BuyingCatalogue.Services.Storage;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Storage;

public static class AzureBlobStorageServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(AzureBlobStorageService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static async Task DownloadAsync_ValidFile_ReturnsFile(
        byte[] fileContents,
        BlobDocument blobDocument,
        [Frozen] Mock<IAzureBlobContainerClientFactory> clientFactory,
        Mock<Response<BlobDownloadInfo>> blobDownloadResponse,
        Mock<BlobContainerClient> blobContainerClient,
        Mock<BlobClient> blobClient,
        AzureBlobStorageService service)
    {
        var fileStream = new MemoryStream(fileContents);
        var blobDownloadInfo = BlobsModelFactory.BlobDownloadInfo(content: fileStream);

        blobDownloadResponse
            .SetupGet(x => x.Value)
            .Returns(blobDownloadInfo);

        blobClient.Setup(x => x.DownloadAsync())
            .ReturnsAsync(blobDownloadResponse.Object);

        blobContainerClient
            .Setup(x => x.GetBlobClient(blobDocument.DocumentName))
            .Returns(blobClient.Object);

        clientFactory
            .Setup(x => x.GetBlobContainerClient(blobDocument.ContainerName))
            .Returns(blobContainerClient.Object);

        var result = await service.DownloadAsync(blobDocument);

        result.ToArray().Should().BeEquivalentTo(fileContents);
    }

    [Theory]
    [CommonAutoData]
    public static async Task DownloadAsync_InvalidFile_ReturnsNull(
        BlobDocument blobDocument,
        [Frozen] Mock<IAzureBlobContainerClientFactory> clientFactory,
        Mock<BlobContainerClient> blobContainerClient,
        Mock<BlobClient> blobClient,
        AzureBlobStorageService service)
    {
        blobClient.Setup(x => x.DownloadAsync())
            .Throws(new RequestFailedException(string.Empty));

        blobContainerClient
            .Setup(x => x.GetBlobClient(blobDocument.DocumentName))
            .Returns(blobClient.Object);

        clientFactory
            .Setup(x => x.GetBlobContainerClient(blobDocument.ContainerName))
            .Returns(blobContainerClient.Object);

        var result = await service.DownloadAsync(blobDocument);

        result.Should().BeNull();
    }

    [Theory]
    [CommonAutoData]
    public static async Task UploadAsync_ExistingFile_DoesNotOverwrite(
        BlobDocument blobDocument,
        byte[] fileContents,
        [Frozen] Mock<IAzureBlobContainerClientFactory> clientFactory,
        Mock<Response<bool>> existsResponse,
        Mock<BlobContainerClient> blobContainerClient,
        Mock<BlobClient> blobClient,
        AzureBlobStorageService service)
    {
        existsResponse
            .SetupGet(x => x.Value)
            .Returns(true);

        blobClient
            .Setup(x => x.ExistsAsync(default))
            .ReturnsAsync(existsResponse.Object);

        blobContainerClient
            .Setup(x => x.GetBlobClient(blobDocument.DocumentName))
            .Returns(blobClient.Object);

        clientFactory
            .Setup(x => x.GetBlobContainerClient(blobDocument.ContainerName))
            .Returns(blobContainerClient.Object);

        await service.UploadAsync(blobDocument, new MemoryStream(fileContents));

        blobClient.VerifyAll();
        blobClient.VerifyNoOtherCalls();
    }

    [Theory]
    [CommonAutoData]
    public static async Task UploadAsync_NewFile_Uploads(
        BlobDocument blobDocument,
        byte[] fileContents,
        [Frozen] Mock<IAzureBlobContainerClientFactory> clientFactory,
        Mock<Response<bool>> existsResponse,
        Mock<BlobContainerClient> blobContainerClient,
        Mock<BlobClient> blobClient,
        AzureBlobStorageService service)
    {
        var actualContents = new MemoryStream(fileContents);
        Stream submittedContents = null;

        existsResponse
            .SetupGet(x => x.Value)
            .Returns(false);

        blobClient
            .Setup(x => x.ExistsAsync(default))
            .ReturnsAsync(existsResponse.Object);

        blobClient
            .Setup(x => x.UploadAsync(It.IsAny<Stream>()))
            .Callback<Stream>(stream => submittedContents = stream)
            .ReturnsAsync((Response<BlobContentInfo>)null);

        blobContainerClient
            .Setup(x => x.GetBlobClient(blobDocument.DocumentName))
            .Returns(blobClient.Object);

        clientFactory
            .Setup(x => x.GetBlobContainerClient(blobDocument.ContainerName))
            .Returns(blobContainerClient.Object);

        await service.UploadAsync(blobDocument, actualContents);

        blobClient.VerifyAll();

        submittedContents.Should().NotBeNull();
        submittedContents.Should().BeSameAs(submittedContents);
    }
}
