using System;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Extensions;

public static class EnumExtensionsTests
{
    [Theory]
    [InlineData(0, SupportedIntegrations.Im1)]
    [InlineData(1, SupportedIntegrations.GpConnect)]
    [InlineData(2, SupportedIntegrations.NhsApp)]
    public static void ToIntegrationId_ReturnsExpected(
        int id,
        SupportedIntegrations expectedId) => id.ToIntegrationId().Should().Be(expectedId);

    [Theory]
    [InlineData(3)]
    [InlineData(15)]
    [InlineData(57)]
    [InlineData(314)]
    [InlineData(5136)]
    public static void ToIntegrationId_OutOfRange_ThrowsArgumentOutOfRangeException(int id) => FluentActions
        .Invoking(() => id.ToIntegrationId())
        .Should()
        .Throw<ArgumentOutOfRangeException>();
}
