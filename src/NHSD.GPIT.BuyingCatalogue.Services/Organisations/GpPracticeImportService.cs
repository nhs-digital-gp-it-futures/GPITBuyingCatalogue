using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Organisations.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Organisations
{
    public class GpPracticeImportService : IGpPracticeImportService
    {
        private const string TruncateStatement = "TRUNCATE TABLE [organisations].[GpPracticeSize]";

        private readonly BuyingCatalogueDbContext dbContext;
        private readonly IGpPracticeProvider gpPracticeProvider;

        public GpPracticeImportService(
            BuyingCatalogueDbContext dbContext,
            IGpPracticeProvider gpPracticeProvider)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
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
            // SQLite doesn't support TRUNCATE or schemas.
            if (dbContext.Database.IsSqlServer())
            {
                await dbContext.Database.ExecuteSqlRawAsync(TruncateStatement);
            }
            else
            {
                // Slower method that only runs for tests against in-memory db
                dbContext.GpPracticeSizes.RemoveRange(dbContext.GpPracticeSizes);
            }

            await dbContext.GpPracticeSizes.AddRangeAsync(gpPractices.Select(x => new GpPracticeSize
            {
                OdsCode = x.CODE,
                NumberOfPatients = x.NUMBER_OF_PATIENTS,
                ExtractDate = x.EXTRACT_DATE,
            }));

            await dbContext.SaveChangesAsync();

            return new ImportGpPracticeListResult
            {
                Outcome = ImportGpPracticeListOutcome.Success,
                TotalRecords = dbContext.GpPracticeSizes.Count(),
                TotalRecordsUpdated = dbContext.GpPracticeSizes.Count(),
                ExtractDate = dbContext.GpPracticeSizes.FirstOrDefault()?.ExtractDate ?? DateTime.Today,
            };
        }
    }
}
