using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;
using NHSD.GPIT.BuyingCatalogue.Services.Storage;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Storage;

public static class AzureBlobStorageServiceTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(AzureBlobStorageService).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static async Task DownloadAsync_ValidFile_ReturnsFile(
        byte[] fileContents,
        BlobDocument blobDocument,
        [Frozen] IAzureBlobContainerClientFactory clientFactory,
        Response<BlobDownloadInfo> blobDownloadResponse,
        AzureBlobStorageService service)
    {
        var blobContainerClient = Substitute.For<BlobContainerClient>();
        var blobClient = Substitute.For<BlobClient>();
        var fileStream = new MemoryStream(fileContents);
        var blobDownloadInfo = BlobsModelFactory.BlobDownloadInfo(content: fileStream);

        blobDownloadResponse.Value.Returns(blobDownloadInfo);

        blobClient.DownloadAsync().Returns(blobDownloadResponse);

        blobContainerClient.GetBlobClient(blobDocument.DocumentName).Returns(blobClient);

        clientFactory.GetBlobContainerClient(blobDocument.ContainerName).Returns(blobContainerClient);

        var result = await service.DownloadAsync(blobDocument);

        result.ToArray().Should().BeEquivalentTo(fileContents);
    }

    [Theory]
    [MockAutoData]
    public static async Task DownloadAsync_InvalidFile_ReturnsNull(
        BlobDocument blobDocument,
        [Frozen] IAzureBlobContainerClientFactory clientFactory,
        AzureBlobStorageService service)
    {
        var blobContainerClient = Substitute.For<BlobContainerClient>();
        var blobClient = Substitute.For<BlobClient>();
        blobClient.DownloadAsync().Returns(Task.FromException<Response<BlobDownloadInfo>>(new RequestFailedException(string.Empty)));

        blobContainerClient.GetBlobClient(blobDocument.DocumentName).Returns(blobClient);

        clientFactory.GetBlobContainerClient(blobDocument.ContainerName).Returns(blobContainerClient);

        var result = await service.DownloadAsync(blobDocument);

        result.Should().BeNull();
    }

    [Theory]
    [MockAutoData]
    public static async Task UploadAsync_ExistingFile_DoesNotOverwrite(
        BlobDocument blobDocument,
        byte[] fileContents,
        [Frozen] IAzureBlobContainerClientFactory clientFactory,
        Response<bool> existsResponse,
        AzureBlobStorageService service)
    {
        var blobContainerClient = Substitute.For<BlobContainerClient>();
        var blobClient = Substitute.For<BlobClient>();
        existsResponse.Value.Returns(true);

        blobClient.ExistsAsync(default).Returns(existsResponse);

        blobContainerClient.GetBlobClient(blobDocument.DocumentName).Returns(blobClient);

        clientFactory.GetBlobContainerClient(blobDocument.ContainerName).Returns(blobContainerClient);

        await service.UploadAsync(blobDocument, new MemoryStream(fileContents));

        await blobClient.Received().ExistsAsync();
    }

    [Theory]
    [MockAutoData]
    public static async Task UploadAsync_NewFile_Uploads(
        BlobDocument blobDocument,
        byte[] fileContents,
        [Frozen] IAzureBlobContainerClientFactory clientFactory,
        Response<bool> existsResponse,
        AzureBlobStorageService service)
    {
        var blobContainerClient = Substitute.For<BlobContainerClient>();
        var blobClient = Substitute.For<BlobClient>();
        var actualContents = new MemoryStream(fileContents);
        Stream submittedContents = null;

        existsResponse.Value.Returns(false);

        blobClient.ExistsAsync(default).Returns(existsResponse);

        await blobClient.UploadAsync(Arg.Do<Stream>(stream => submittedContents = stream));

        blobContainerClient.GetBlobClient(blobDocument.DocumentName).Returns(blobClient);

        clientFactory.GetBlobContainerClient(blobDocument.ContainerName).Returns(blobContainerClient);

        await service.UploadAsync(blobDocument, actualContents);

        await blobClient.Received().ExistsAsync();
        await blobClient.Received().UploadAsync(Arg.Any<Stream>());

        submittedContents.Should().NotBeNull();
        submittedContents.Should().BeSameAs(submittedContents);
    }
}
