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
        => await GetSolutionStandardsBaseQuery(solutionId).ToListAsync();

    public async Task<StandardComplianceModel> GetSolutionStandard(CatalogueItemId solutionId, string standardId)
    {
        var solutionStandards = await GetSolutionStandardsBaseQuery(solutionId).ToListAsync();

        return solutionStandards.SingleOrDefault(x => x.Id == standardId);
    }

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
            context.SolutionStandards.Add(new SolutionStandard(standardId, compliance) { SolutionId = solutionId });
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

    private IQueryable<StandardComplianceModel> GetSolutionStandardsBaseQuery(CatalogueItemId solutionId)
    {
        var standards = context.CatalogueItemCapabilities
            .AsNoTracking()
            .Where(x => x.CatalogueItemId == solutionId)
            .SelectMany(x => x.Capability.StandardCapabilities)
            .Select(x => x.Standard)
            .Union(
                context.Standards
                    .AsNoTracking()
                    .Where(x => x.StandardType == StandardType.Overarching))
            .Distinct();

        var solutionStandards = context.SolutionStandards
            .AsNoTracking()
            .Where(x => x.SolutionId == solutionId);

        return standards.LeftJoin(
            solutionStandards,
            standard => standard.Id,
            solutionStandard => solutionStandard.StandardId,
            (standard, solutionStandard) => new StandardComplianceModel(
                standard,
                solutionStandard != null ? solutionStandard.Status : StandardCompliance.NotYetSelected));
    }
}
