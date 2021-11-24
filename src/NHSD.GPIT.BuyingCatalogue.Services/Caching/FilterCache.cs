using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Caching;

namespace NHSD.GPIT.BuyingCatalogue.Services.Caching
{
    public sealed class FilterCache : IFilterCache
    {
        private readonly IMemoryCache memoryCache;
        private readonly FilterCacheKeysSettings filterCacheKeySettings;
        private readonly MemoryCacheEntryOptions memoryCacheEntryOptions;

        public FilterCache(
            IMemoryCache memoryCache,
            FilterCacheKeysSettings filterCacheKeySettings)
        {
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            this.filterCacheKeySettings = filterCacheKeySettings ?? throw new ArgumentNullException(nameof(filterCacheKeySettings));

            memoryCacheEntryOptions = new MemoryCacheEntryOptions()
                .AddExpirationToken(new CancellationChangeToken(CancellationToken.Token));
        }

        private CancellationTokenSource CancellationToken =>
           memoryCache.GetOrCreate(filterCacheKeySettings.CancellationSourceKey, cacheEntry =>
           {
               return new CancellationTokenSource();
           });

        public void Remove(string filterKey) => memoryCache.Remove(GetCacheKey(filterKey));

        public void Remove(IEnumerable<string> filterKeys)
        {
            if (filterKeys is null)
                throw new ArgumentNullException(nameof(filterKeys));

            foreach (var filterKey in filterKeys)
            {
                Remove(filterKey);
            }
        }

        public void RemoveAll()
        {
            var cancellationToken = memoryCache.Get<CancellationTokenSource>(filterCacheKeySettings.CancellationSourceKey);
            cancellationToken.Cancel();
        }

        public void Set(string filterKey, string content)
        {
            var cacheKey = GetCacheKey(filterKey);

            memoryCache.Set(cacheKey, content, memoryCacheEntryOptions);
        }

        public string Get(string filterKey)
        {
            memoryCache.TryGetValue(GetCacheKey(filterKey), out string content);

            return content;
        }

        private string GetCacheKey(string filterKey) => $"{filterCacheKeySettings.FrameworkFilterKey}{filterKey}";
    }
}
