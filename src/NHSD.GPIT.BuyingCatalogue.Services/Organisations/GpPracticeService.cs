using System;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Caching;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Organisations
{
    public sealed class GpPracticeService : IGpPracticeService
    {
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly ILogWrapper<GpPracticeService> logger;
        private readonly IGpPracticeCache gpPracticeCache;
        private readonly IGpPracticeImportService gpPracticeImportService;

        public GpPracticeService(
            BuyingCatalogueDbContext dbContext,
            ILogWrapper<GpPracticeService> logger,
            IGpPracticeCache gpPracticeCache,
            IGpPracticeImportService gpPracticeImportService)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.gpPracticeCache = gpPracticeCache ?? throw new ArgumentNullException(nameof(gpPracticeCache));
            this.gpPracticeImportService = gpPracticeImportService ?? throw new ArgumentNullException(nameof(gpPracticeImportService));
        }

        public async Task ImportGpPracticeData(Uri csvUri, string emailAddress)
        {
            if (csvUri == null)
                throw new ArgumentNullException(nameof(csvUri));

            if (string.IsNullOrWhiteSpace(emailAddress))
                throw new ArgumentNullException(nameof(emailAddress));

            var result = await gpPracticeImportService.PerformImport(csvUri);

            SendConfirmationEmail(result, emailAddress);
        }

        public async Task<int?> GetNumberOfPatients(string odsCode)
        {
            var cachedValue = gpPracticeCache.Get(odsCode);

            if (cachedValue.HasValue)
            {
                return cachedValue.Value == -1
                    ? null
                    : cachedValue;
            }

            var dbItem = await dbContext.GpPracticeSizes.FindAsync(odsCode);

            if (dbItem == null)
                gpPracticeCache.Set(odsCode, -1);
            else
                gpPracticeCache.Set(odsCode, dbItem.NumberOfPatients);

            return dbItem?.NumberOfPatients;
        }

        private void SendConfirmationEmail(ImportGpPracticeListResult result, string emailAddress)
        {
            // TODO: Replace with email to admin user
            switch (result.Outcome)
            {
                case ImportGpPracticeListOutcome.Success:
                    logger.LogInformation(result.ToString());
                    break;
                case ImportGpPracticeListOutcome.CannotReadInputFile:
                    logger.LogInformation(result.ToString());
                    break;
                case ImportGpPracticeListOutcome.WrongFormat:
                    logger.LogInformation(result.ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(result));
            }
        }
    }
}
