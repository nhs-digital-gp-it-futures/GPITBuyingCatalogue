using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Storage;

public static class BlobDocumentTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(BlobDocument).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [CommonAutoData]
    public static void Construct_AssignsPropertiesAsExpected(
        string containerName,
        string documentName)
    {
        var blobDocument = new BlobDocument(containerName, documentName);

        blobDocument.ContainerName.Should().Be(containerName);
        blobDocument.DocumentName.Should().Be(documentName);
    }
}
