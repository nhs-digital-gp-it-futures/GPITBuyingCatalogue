using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Caching;

namespace NHSD.GPIT.BuyingCatalogue.Services.Caching
{
    public sealed class GpPracticeCache : IGpPracticeCache
    {
        private readonly IMemoryCache memoryCache;
        private readonly GpPracticeCacheKeysSettings settings;
        private readonly MemoryCacheEntryOptions options;

        public GpPracticeCache(
            IMemoryCache memoryCache,
            GpPracticeCacheKeysSettings settings)
        {
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            this.settings = settings ?? throw new ArgumentNullException(nameof(settings));

            var tokenSource = memoryCache.GetOrCreate(
                settings.CancellationSourceKey,
                _ => new CancellationTokenSource());

            var token = new CancellationChangeToken(tokenSource.Token);

            options = new MemoryCacheEntryOptions().AddExpirationToken(token);
        }

        public int? Get(string odsCode)
        {
            return memoryCache.TryGetValue(GetCacheKey(odsCode), out int numberOfPatients)
                ? numberOfPatients
                : null;
        }

        public void Set(string odsCode, int numberOfPatients)
        {
            memoryCache.Set(GetCacheKey(odsCode), numberOfPatients, options);
        }

        public void RemoveAll() => memoryCache.Get<CancellationTokenSource>(settings.CancellationSourceKey).Cancel();

        private string GetCacheKey(string odsCode) => $"{settings.GpPracticeKey}{odsCode}";
    }
}
