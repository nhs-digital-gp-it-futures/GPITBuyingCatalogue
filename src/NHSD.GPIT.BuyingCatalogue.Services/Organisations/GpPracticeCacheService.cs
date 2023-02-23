using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations;

namespace NHSD.GPIT.BuyingCatalogue.Services.Organisations
{
    public class GpPracticeCacheService : IGpPracticeCacheService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        private Dictionary<string, int> practiceSizes;

        public GpPracticeCacheService(
            BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

            Refresh();
        }

        public int? GetNumberOfPatients(string odsCode)
        {
            if (practiceSizes.TryGetValue(odsCode, out var value))
            {
                return value;
            }

            return null;
        }

        public void Refresh()
        {
            practiceSizes = dbContext.GpPracticeSizes
                .AsNoTracking()
                .ToDictionary(
                    x => x.OdsCode,
                    x => x.NumberOfPatients);
        }
    }
}
