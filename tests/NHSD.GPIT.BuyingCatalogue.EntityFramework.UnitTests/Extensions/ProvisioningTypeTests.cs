using System;
using AutoFixture.NUnit3;
using FluentAssertions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Extensions;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NUnit.Framework;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.UnitTests.Extensions
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    internal static class ProvisioningTypeTests
    {
        [TestCase(ProvisioningType.Patient, TimeUnit.PerMonth)]
        [TestCase(ProvisioningType.Declarative, TimeUnit.PerYear)]
        public static void InferEstimationPeriod_ReturnsExpectedValue(
            ProvisioningType provisioningType,
            TimeUnit expectedTimeUnit)
        {
            var estimationPeriod = provisioningType.InferEstimationPeriod(null);

            estimationPeriod.Should().Be(expectedTimeUnit);
        }

        [Test]
        [AutoData]
        public static void InferEstimationPeriod_ReturnsExpectedValue(TimeUnit expectedEstimationPeriod)
        {
            var actualEstimationPeriod = ProvisioningType.OnDemand.InferEstimationPeriod(expectedEstimationPeriod);

            actualEstimationPeriod.Should().Be(expectedEstimationPeriod);
        }

        [Test]
        public static void InferEstimationPeriod_InvalidProvisioningType_ThrowsException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => ((ProvisioningType)0).InferEstimationPeriod(null));
        }
    }
}
