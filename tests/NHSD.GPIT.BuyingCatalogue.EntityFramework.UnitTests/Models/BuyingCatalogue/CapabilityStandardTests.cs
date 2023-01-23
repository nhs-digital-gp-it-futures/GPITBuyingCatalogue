﻿using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.UnitTest.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.BuyingCatalogue;

public static class CapabilityStandardTests
{
    [Theory]
    [CommonAutoData]
    public static void Construct_AssignsPropertiesAsExpected(
        string standardId,
        int capabilityId)
    {
        var standardCapability = new StandardCapability(standardId, capabilityId);

        standardCapability.StandardId.Should().Be(standardId);
        standardCapability.CapabilityId.Should().Be(capabilityId);
    }
}
