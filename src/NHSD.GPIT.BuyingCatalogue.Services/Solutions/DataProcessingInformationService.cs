using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions;

public class DataProcessingInformationService(BuyingCatalogueDbContext dbContext) : IDataProcessingInformationService
{
    private readonly BuyingCatalogueDbContext dbContext =
        dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task<Solution> GetSolutionWithDataProcessingInformation(CatalogueItemId solutionId) => await dbContext
        .Solutions
        .Include(x => x.CatalogueItem)
        .Include(x => x.DataProcessingInformation)
        .Include(x => x.DataProcessingInformation.Details)
        .Include(x => x.DataProcessingInformation.Location)
        .Include(x => x.DataProcessingInformation.Officer)
        .Include(x => x.DataProcessingInformation.SubProcessors)
        .ThenInclude(x => x.Details)
        .AsNoTracking()
        .AsSplitQuery()
        .FirstOrDefaultAsync(x => x.CatalogueItemId == solutionId);
}
