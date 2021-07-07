using System;
using AutoFixture.Xunit2;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Extensions
{
    public static class ProvisioningTypeTests
    {
        [Theory]
        [InlineData(ProvisioningType.Patient, TimeUnit.PerMonth)]
        [InlineData(ProvisioningType.Declarative, TimeUnit.PerYear)]
        public static void InferEstimationPeriod_DeclarativeOrPatient_ReturnsExpectedValue(
            ProvisioningType provisioningType,
            TimeUnit expectedTimeUnit)
        {
            var estimationPeriod = provisioningType.InferEstimationPeriod(null);

            estimationPeriod.Should().Be(expectedTimeUnit);
        }

        [Theory]
        [AutoData]
        public static void InferEstimationPeriod_OnDemand_ReturnsExpectedValue(TimeUnit expectedEstimationPeriod)
        {
            var actualEstimationPeriod = ProvisioningType.OnDemand.InferEstimationPeriod(expectedEstimationPeriod);

            actualEstimationPeriod.Should().Be(expectedEstimationPeriod);
        }

        [Fact]
        public static void InferEstimationPeriod_InvalidProvisioningType_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ((ProvisioningType)0).InferEstimationPeriod(null));
        }
    }
}
