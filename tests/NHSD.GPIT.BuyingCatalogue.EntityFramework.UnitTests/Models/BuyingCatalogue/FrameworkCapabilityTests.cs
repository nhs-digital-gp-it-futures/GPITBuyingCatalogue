using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.Attributes;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.BuyingCatalogue;

public static class FrameworkCapabilityTests
{
    [Theory]
    [MockAutoData]
    public static void Construct_SetsPropertiesAsExpected(
        string frameworkId,
        int capabilityId)
    {
        var frameworkCapability = new FrameworkCapability(frameworkId, capabilityId);

        frameworkCapability.FrameworkId.Should().Be(frameworkId);
        frameworkCapability.CapabilityId.Should().Be(capabilityId);
    }
}
