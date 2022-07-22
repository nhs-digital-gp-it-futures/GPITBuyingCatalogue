using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Orders.Contracts;

namespace NHSD.GPIT.BuyingCatalogue.Services.Orders.Contracts;

public class DataProcessingPlanService : IDataProcessingPlanService
{
    private readonly BuyingCatalogueDbContext dbContext;

    public DataProcessingPlanService(
        BuyingCatalogueDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<DataProcessingPlan> CreateDataProcessingPlan()
    {
        var dataProcessingPlan = new DataProcessingPlan { IsDefault = false, };

        dbContext.DataProcessingPlans.Add(dataProcessingPlan);

        await dbContext.SaveChangesAsync();

        return dataProcessingPlan;
    }

    public async Task<DataProcessingPlan> GetDefaultDataProcessingPlan() =>
        await dbContext
            .DataProcessingPlans
            .Include(d => d.Steps)
            .FirstOrDefaultAsync(d => d.IsDefault);
}
