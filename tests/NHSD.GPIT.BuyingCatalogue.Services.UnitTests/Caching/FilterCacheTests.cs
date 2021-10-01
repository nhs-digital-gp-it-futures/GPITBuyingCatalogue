using System;
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
    public static class FilterCacheTests
    {
        [Theory]
        [CommonAutoData]
        public static void Set_AddsToCache(
            string filterKey,
            string content,
            DateTime expiration,
            [Frozen] Mock<IMemoryCache> memoryCache,
            [Frozen] FilterCacheKeySettings filterCacheKeySettings,
            FilterCache filterCache)
        {
            var expectedCacheKey = $"{filterCacheKeySettings.FilterCacheKey}{filterKey}";

            filterCache.Set(filterKey, content, expiration);

            memoryCache.Verify(m => m.CreateEntry(expectedCacheKey));
        }

        [Theory]
        [CommonAutoData]
        public static void Remove_RemovesFromCache(
            string filterKey,
            [Frozen] Mock<IMemoryCache> memoryCache,
            [Frozen] FilterCacheKeySettings filterCacheKeySettings,
            FilterCache filterCache)
        {
            var expectedCacheKey = $"{filterCacheKeySettings.FilterCacheKey}{filterKey}";

            filterCache.Remove(filterKey);

            memoryCache.Verify(m => m.Remove(expectedCacheKey));
        }

        [Theory]
        [CommonAutoData]
        public static void TryGet_WithEntry_ReturnsExpectedContent(
            string filterKey,
            string content,
            [Frozen] Mock<IMemoryCache> memoryCache,
            [Frozen] FilterCacheKeySettings filterCacheKeySettings,
            FilterCache filterCache)
        {
            var cacheKey = $"{filterCacheKeySettings.FilterCacheKey}{filterKey}";

            object cacheContent = content;
            memoryCache.Setup(m => m.TryGetValue(cacheKey, out cacheContent))
                .Returns(true);

            var result = filterCache.TryGet(filterKey, out var actualContent);

            result
                .Should()
                .BeTrue();

            actualContent
                .Should()
                .Be(content);
        }

        [Theory]
        [CommonAutoData]
        public static void TryGet_WithInvalidKey_ReturnsFalse(
            string filterKey,
            [Frozen] Mock<IMemoryCache> memoryCache,
            FilterCache filterCache)
        {
            object cacheContent = null;
            memoryCache.Setup(m => m.TryGetValue(It.IsAny<string>(), out cacheContent))
                .Returns(false);

            var result = filterCache.TryGet(filterKey, out var actualContent);

            result
                .Should()
                .BeFalse();
        }
    }
}
