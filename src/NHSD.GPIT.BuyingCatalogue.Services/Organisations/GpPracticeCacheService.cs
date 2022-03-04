using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Caching;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Organisations
{
    public class GpPracticeCacheService : IGpPracticeCacheService
    {
        public const int EntryNotFound = -1;

        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IGpPracticeCache gpPracticeCache;

        public GpPracticeCacheService(
            BuyingCatalogueDbContext dbContext,
            IGpPracticeCache gpPracticeCache)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.gpPracticeCache = gpPracticeCache ?? throw new ArgumentNullException(nameof(gpPracticeCache));
        }

        public async Task<int?> GetNumberOfPatients(string odsCode)
        {
            var cachedValue = gpPracticeCache.Get(odsCode);

            if (cachedValue.HasValue)
            {
                return cachedValue.Value == EntryNotFound
                    ? null
                    : cachedValue;
            }

            var dbItem = await dbContext.GpPracticeSizes.FindAsync(odsCode);

            gpPracticeCache.Set(odsCode, dbItem?.NumberOfPatients ?? EntryNotFound);

            return dbItem?.NumberOfPatients;
        }
    }
}
