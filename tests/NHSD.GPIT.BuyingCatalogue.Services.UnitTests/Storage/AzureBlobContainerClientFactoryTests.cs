using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using AutoFixture.Xunit2;
using Azure.Storage.Blobs;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.Services.Storage;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using NSubstitute;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Storage;

public static class AzureBlobContainerClientFactoryTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(
            new CompositeCustomization(
                new AutoNSubstituteCustomization(),
                new BlobServiceClientSubstituteCustomization()));
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(AzureBlobContainerClientFactory).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static void GetBlobContainerClient_ReturnsClient(
        string containerName,
        [Frozen] BlobServiceClient blobServiceClient,
        AzureBlobContainerClientFactory factory)
    {
        var blobContainerClient = Substitute.For<BlobContainerClient>();
        blobServiceClient.GetBlobContainerClient(containerName)
            .Returns(blobContainerClient);

        var client = factory.GetBlobContainerClient(containerName);

        client.Should().NotBeNull();
    }
}
