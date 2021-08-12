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
        private readonly string serviceRecipientCacheKey;
        private readonly IMemoryCache memoryCache;
        private readonly MemoryCacheEntryOptions memoryCacheOptions;

        public MemoryCacheHandler(
            IMemoryCache memoryCache,
            string odsCode)
        {
            this.memoryCache = memoryCache;

            serviceRecipientCacheKey = $"ServiceRecipients-ODS-{odsCode}";

            memoryCacheOptions =
                new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddSeconds(DefaultCacheDuration));

            SetServiceRecipients(ServiceRecipientsSeedData.GetServiceRecipients);
        }

        public IEnumerable<ServiceRecipient> GetServiceRecipients()
        {
            memoryCache.TryGetValue(
                serviceRecipientCacheKey,
                out IEnumerable<ServiceRecipient> cachedResults);

            return cachedResults;
        }

        public void SetServiceRecipients(IEnumerable<ServiceRecipient> serviceRecipients)
        {
            memoryCache.Remove(serviceRecipientCacheKey);

            memoryCache.Set(serviceRecipientCacheKey, serviceRecipients, memoryCacheOptions);
        }
    }
}
