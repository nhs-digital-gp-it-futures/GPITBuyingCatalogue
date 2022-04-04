using System.Threading;
using AutoFixture.Xunit2;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.Services.Caching;
using NHSD.GPIT.BuyingCatalogue.Test.Framework.AutoFixtureCustomisations;
using Xunit;

namespace NHSD.GPIT.BuyingCatalogue.Services.UnitTests.Caching
{
    public static class GpPracticeCacheTests
    {
        [Theory]
        [CommonAutoData]
        public static void Set_AddsToCache(
            string odsCode,
            int numberOfPatients,
            [Frozen] Mock<IMemoryCache> memoryCache,
            [Frozen] GpPracticeCacheKeysSettings settings,
            GpPracticeCache systemUnderTest)
        {
            var cacheKey = $"{settings.GpPracticeKey}{odsCode}";

            systemUnderTest.Set(odsCode, numberOfPatients);

            memoryCache.Verify(m => m.CreateEntry(cacheKey));
        }

        [Theory]
        [CommonAutoData]
        public static void TryGet_WithEntry_ReturnsExpectedContent(
            string odsCode,
            int numberOfPatients,
            [Frozen] Mock<IMemoryCache> memoryCache,
            [Frozen] GpPracticeCacheKeysSettings settings,
            GpPracticeCache systemUnderTest)
        {
            var cacheKey = $"{settings.GpPracticeKey}{odsCode}";

            object cacheContent = numberOfPatients;

            memoryCache
                .Setup(m => m.TryGetValue(cacheKey, out cacheContent))
                .Returns(true);

            var result = systemUnderTest.Get(odsCode);

            result.Should().Be(numberOfPatients);
        }

        [Theory]
        [CommonAutoData]
        public static void TryGet_WithInvalidKey_ReturnsNull(
            string odsCode,
            [Frozen] Mock<IMemoryCache> memoryCache,
            GpPracticeCache systemUnderTest)
        {
            object cacheContent = null;

            memoryCache
                .Setup(m => m.TryGetValue(It.IsAny<string>(), out cacheContent))
                .Returns(false);

            var result = systemUnderTest.Get(odsCode);

            result.Should().BeNull();
        }
    }
}
