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
        => await context.CatalogueItemCapabilities.Where(x => x.CatalogueItemId == solutionId)
            .SelectMany(x => x.Capability.StandardCapabilities)
            .Select(x => x.Standard)
            .Union(context.Standards.Where(x => x.StandardType == StandardType.Overarching))
            .Distinct()
            .AsNoTracking()
            .Select(x => new StandardComplianceModel(
                x,
                context.InProgressSolutionStandards.Any(y => y.SolutionId == solutionId && y.StandardId == x.Id)))
            .ToListAsync();

    public async Task<StandardComplianceModel> GetSolutionStandard(CatalogueItemId solutionId, string standardId) =>
        await context.Standards.Where(x => x.Id == standardId)
            .AsNoTracking()
            .Select(x => new StandardComplianceModel(
                x,
                context.InProgressSolutionStandards.Any(y => y.SolutionId == solutionId && y.StandardId == x.Id)))
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<Standard>> GetInProgressStandards(CatalogueItemId solutionId) => await context
        .InProgressSolutionStandards.Where(x => x.SolutionId == solutionId)
        .Select(x => x.Standard)
        .AsNoTracking()
        .ToListAsync();

    public async Task SetSolutionStandardStatus(
        CatalogueItemId solutionId,
        string standardId,
        StandardCompliance compliance)
    {
        var inProgressSolutionStandard = await context.InProgressSolutionStandards
            .Where(x => x.SolutionId == solutionId && x.StandardId == standardId)
            .FirstOrDefaultAsync();

        switch (compliance)
        {
            case StandardCompliance.InProgress when inProgressSolutionStandard is not null:
            case StandardCompliance.FullyMet when inProgressSolutionStandard is null:
                return;
            case StandardCompliance.InProgress:
                context.InProgressSolutionStandards.Add(new InProgressSolutionStandard(solutionId, standardId));
                break;
            case StandardCompliance.FullyMet:
                context.InProgressSolutionStandards.Remove(inProgressSolutionStandard);

                var workOffPlans =
                    await context.WorkOffPlans.Where(x => x.SolutionId == solutionId && x.StandardId == standardId)
                        .ToListAsync();

                context.RemoveRange(workOffPlans);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(compliance), compliance, null);
        }

        await context.SaveChangesAsync();
    }
}
