using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using Azure.Storage.Blobs;
using FluentAssertions;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Services.Storage;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Storage;

public static class AzureBlobContainerClientFactoryTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(
            new CompositeCustomization(
                new AutoMoqCustomization(),
                new BlobServiceClientMoqCustomization()));
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(AzureBlobContainerClientFactory).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static void GetBlobContainerClient_ReturnsClient(
        string containerName,
        Mock<BlobContainerClient> blobContainerClient,
        [Frozen] Mock<BlobServiceClient> blobServiceClient,
        AzureBlobContainerClientFactory factory)
    {
        blobServiceClient.Setup(x => x.GetBlobContainerClient(containerName))
            .Returns(blobContainerClient.Object);

        var client = factory.GetBlobContainerClient(containerName);

        client.Should().NotBeNull();
    }
}
