using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Models.CapabilityMappingModels;

public static class Gen2CatalogueItemMappingModelTests
{
    [Fact]
    public static void Constructing_InvalidCatalogueItemId_ThrowsFormatException() => FluentActions
        .Invoking(
            () => new Gen2CatalogueItemMappingModel(
                Guid.NewGuid().ToString(),
                Enumerable.Empty<Gen2CapabilityMappingModel>()))
        .Should()
        .Throw<FormatException>();

    [Theory]
    [MockAutoData]
    public static void Constructing_Valid_SetsPropertiesAsExpected(
        CatalogueItemId catalogueItemId)
    {
        var model = new Gen2CatalogueItemMappingModel(
            catalogueItemId.ToString(),
            Enumerable.Empty<Gen2CapabilityMappingModel>());

        model.Id.Should().Be(catalogueItemId);
        model.Capabilities.Should().BeEmpty();
    }

    [Theory]
    [MockAutoData]
    public static void Constructing_DuplicateCapabilities_SetsPropertiesAsExpected(
        CatalogueItemId catalogueItemId,
        Gen2CapabilityMappingModel capabilityModel)
    {
        var capabilities = new[] { capabilityModel, capabilityModel };

        var model = new Gen2CatalogueItemMappingModel(catalogueItemId.ToString(), capabilities);

        model.Capabilities.Should().ContainSingle();
        model.Capabilities.Should().BeEquivalentTo(new[] { capabilityModel });
    }
}
