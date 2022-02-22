using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.SeedData;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Utils.MemoryCache
{
    public sealed class MemoryCacheHandler
    {
        private const int DefaultCacheDuration = 60;
        private readonly IMemoryCache memoryCache;
        private readonly MemoryCacheEntryOptions memoryCacheOptions;
        private string serviceRecipientCacheKey;

        public MemoryCacheHandler(
            IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;

            memoryCacheOptions =
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddSeconds(DefaultCacheDuration));
        }

        public void InitializeServiceRecipients(string odsCode)
        {
            serviceRecipientCacheKey = $"ServiceRecipients-Identifier-{odsCode}";

            SetServiceRecipients(ServiceRecipientsSeedData.GetServiceRecipients);
        }

        public IEnumerable<ServiceRecipient> GetServiceRecipients()
        {
            memoryCache.TryGetValue(
                serviceRecipientCacheKey,
                out IEnumerable<ServiceRecipient> cachedResults);

            return cachedResults;
        }

        public void Remove(string cacheKey) => memoryCache.Remove(cacheKey);

        private void SetServiceRecipients(IEnumerable<ServiceRecipient> serviceRecipients)
        {
            memoryCache.Remove(serviceRecipientCacheKey);

            memoryCache.Set(serviceRecipientCacheKey, serviceRecipients, memoryCacheOptions);
        }
    }
}
