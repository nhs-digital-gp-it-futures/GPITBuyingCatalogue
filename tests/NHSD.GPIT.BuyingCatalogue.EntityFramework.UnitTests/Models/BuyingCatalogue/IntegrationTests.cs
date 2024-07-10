using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Models.BuyingCatalogue;

public static class IntegrationTests
{
    [Theory]
    [InlineData(SupportedIntegrations.Im1, false)]
    [InlineData(SupportedIntegrations.GpConnect, false)]
    [InlineData(SupportedIntegrations.NhsApp, true)]
    public static void RequiresDescription_SpecifiedId_ReturnsExpected(
        SupportedIntegrations integrationId,
        bool expected)
    {
        var integration = new Integration { Id = integrationId };

        integration.RequiresDescription.Should().Be(expected);
    }
}
