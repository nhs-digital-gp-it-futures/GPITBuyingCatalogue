using AutoFixture;
using AutoFixture.AutoNSubstitute;
using AutoFixture.Idioms;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Storage;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Storage;

public static class BlobDocumentTests
{
    [Fact]
    public static void Constructors_VerifyGuardClauses()
    {
        var fixture = new Fixture().Customize(new AutoNSubstituteCustomization());
        var assertion = new GuardClauseAssertion(fixture);
        var constructors = typeof(BlobDocument).GetConstructors();

        assertion.Verify(constructors);
    }

    [Theory]
    [MockAutoData]
    public static void Construct_AssignsPropertiesAsExpected(
        string containerName,
        string documentName)
    {
        var blobDocument = new BlobDocument(containerName, documentName);

        blobDocument.ContainerName.Should().Be(containerName);
        blobDocument.DocumentName.Should().Be(documentName);
    }
}
