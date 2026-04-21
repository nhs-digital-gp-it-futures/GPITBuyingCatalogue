using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Solutions.Models;

namespace NHSD.GPIT.BuyingCatalogue.Services.Solutions;

public class SolutionStandardsService(BuyingCatalogueDbContext context) : ISolutionStandardsService
{
    public async Task<IEnumerable<StandardComplianceModel>> GetSolutionStandards(CatalogueItemId solutionId)
        => await context.SolutionStandards
            .AsNoTracking()
            .Where(x => x.SolutionId == solutionId)
            .Select(x => new StandardComplianceModel(x.Standard, x.Status))
            .ToListAsync();

    public async Task<StandardComplianceModel> GetSolutionStandard(CatalogueItemId solutionId, string standardId) =>
        await context.SolutionStandards.Where(x => x.StandardId == standardId && x.SolutionId == solutionId)
            .AsNoTracking()
            .Select(x => new StandardComplianceModel(
                x.Standard,
                x.Status))
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<Standard>> GetInProgressStandards(CatalogueItemId solutionId)
        => await context.SolutionStandards
            .AsNoTracking()
            .Where(x => x.SolutionId == solutionId && x.Status == StandardCompliance.InProgress)
            .Select(x => x.Standard)
            .ToListAsync();

    public async Task SetSolutionStandardStatus(
        CatalogueItemId solutionId,
        string standardId,
        StandardCompliance compliance)
    {
        var solutionStandard =
            await context.SolutionStandards.FirstOrDefaultAsync(x =>
                x.StandardId == standardId && x.SolutionId == solutionId);

        if (solutionStandard is null)
        {
            context.SolutionStandards.Add(new SolutionStandard { StandardId = standardId, SolutionId = solutionId, Status = compliance });
            await context.SaveChangesAsync();

            return;
        }

        if (compliance == solutionStandard.Status) return;
        if (compliance == StandardCompliance.FullyMet)
        {
            var workOffPlans = context.WorkOffPlans.Where(x =>
                x.StandardId == standardId && x.SolutionId == solutionId);

            context.WorkOffPlans.RemoveRange(workOffPlans);
        }

        solutionStandard.Status = compliance;

        await context.SaveChangesAsync();
    }
}
