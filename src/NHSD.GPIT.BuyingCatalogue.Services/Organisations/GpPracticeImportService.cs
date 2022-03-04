using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Caching;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Organisations
{
    public class GpPracticeImportService : IGpPracticeImportService
    {
        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IGpPracticeCache gpPracticeCache;
        private readonly IGpPracticeProvider gpPracticeProvider;

        public GpPracticeImportService(
            BuyingCatalogueDbContext dbContext,
            IGpPracticeCache gpPracticeCache,
            IGpPracticeProvider gpPracticeProvider)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.gpPracticeCache = gpPracticeCache ?? throw new ArgumentNullException(nameof(gpPracticeCache));
            this.gpPracticeProvider = gpPracticeProvider ?? throw new ArgumentNullException(nameof(gpPracticeProvider));
        }

        public async Task<ImportGpPracticeListResult> PerformImport(Uri csvUri)
        {
            try
            {
                var gpPracticeData = await gpPracticeProvider.GetGpPractices(csvUri);

                if (gpPracticeData == null)
                {
                    return new ImportGpPracticeListResult
                    {
                        Outcome = ImportGpPracticeListOutcome.CannotReadInputFile,
                    };
                }

                return await Import(gpPracticeData);
            }
            catch (FormatException)
            {
                return new ImportGpPracticeListResult
                {
                    Outcome = ImportGpPracticeListOutcome.WrongFormat,
                };
            }
        }

        private async Task<ImportGpPracticeListResult> Import(IEnumerable<GpPractice> gpPractices)
        {
            var total = 0;
            var updated = 0;
            var extractDate = DateTime.MinValue;

            foreach (var gpPractice in gpPractices)
            {
                total++;

                if (extractDate == DateTime.MinValue)
                {
                    extractDate = gpPractice.EXTRACT_DATE;
                }

                var current = await dbContext.GpPracticeSizes.FindAsync(gpPractice.CODE);

                if (current == null)
                {
                    dbContext.GpPracticeSizes.Add(new GpPracticeSize
                    {
                        OdsCode = gpPractice.CODE,
                        NumberOfPatients = gpPractice.NUMBER_OF_PATIENTS,
                        ExtractDate = gpPractice.EXTRACT_DATE,
                    });

                    updated++;
                }
                else
                {
                    if (current.ExtractDate >= gpPractice.EXTRACT_DATE)
                        continue;

                    current.NumberOfPatients = gpPractice.NUMBER_OF_PATIENTS;
                    current.ExtractDate = gpPractice.EXTRACT_DATE;

                    updated++;
                }
            }

            await dbContext.SaveChangesAsync();
            gpPracticeCache.RemoveAll();

            return new ImportGpPracticeListResult
            {
                Outcome = ImportGpPracticeListOutcome.Success,
                TotalRecords = total,
                TotalRecordsUpdated = updated,
                ExtractDate = extractDate,
            };
        }
    }
}
