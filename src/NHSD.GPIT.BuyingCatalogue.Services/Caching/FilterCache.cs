using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Caching;

namespace NHSD.GPIT.BuyingCatalogue.Services.Caching
{
    public sealed class FilterCache : IFilterCache
    {
        private readonly IMemoryCache memoryCache;
        private readonly FilterCacheKeySettings filterCacheKeySettings;

        public FilterCache(
            IMemoryCache memoryCache,
            FilterCacheKeySettings filterCacheKeySettings)
        {
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            this.filterCacheKeySettings = filterCacheKeySettings ?? throw new ArgumentNullException(nameof(filterCacheKeySettings));
        }

        public void Remove(string filterKey)
            => memoryCache.Remove(GetCacheKey(filterKey));

        public void Remove(IEnumerable<string> filterKeys)
        {
            foreach (var filterKey in filterKeys)
            {
                Remove(filterKey);
            }
        }

        public void Set(string filterKey, string content, DateTime expiration)
            => memoryCache.Set(GetCacheKey(filterKey), content, expiration);

        public string Get(string filterKey)
        {
            memoryCache.TryGetValue(GetCacheKey(filterKey), out string content);

            return content;
        }

        private string GetCacheKey(string filterKey) => $"{filterCacheKeySettings.FilterCacheKey}{filterKey}";
    }
}
