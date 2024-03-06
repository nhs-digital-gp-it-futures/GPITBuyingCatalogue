using System;
using System.Linq;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.CapabilitiesMappingModels;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.UnitTests.Models.CapabilityMappingModels;

public static class Gen2CapabilityMappingModelTests
{
    [Fact]
    public static void Constructing_WithInvalidCapabilityReference_ThrowsArgumentException() =>
        FluentActions.Invoking(() => new Gen2CapabilityMappingModel("INVALID", Enumerable.Empty<string>()))
            .Should()
            .Throw<ArgumentException>("Not a valid Capability Reference");

    [Fact]
    public static void Constructing_WithValidCapabilityReference_SetsPropertiesAsExpected()
    {
        const string capabilityRef = "C27";

        var model = new Gen2CapabilityMappingModel(capabilityRef, Enumerable.Empty<string>());

        model.CapabilityId.Should().Be(27);
        model.CapabilityRef.Should().Be(capabilityRef);
        model.Epics.Should().BeEmpty();
    }
}
