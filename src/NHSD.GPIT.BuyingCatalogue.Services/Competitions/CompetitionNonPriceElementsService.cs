using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Competitions;

public class CompetitionNonPriceElementsService : ICompetitionNonPriceElementsService
{
    private readonly BuyingCatalogueDbContext dbContext;

    public CompetitionNonPriceElementsService(
        BuyingCatalogueDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task DeleteNonPriceElement(string internalOrgId, int competitionId, NonPriceElement nonPriceElement)
    {
        var competition = await dbContext.Competitions.Include(x => x.NonPriceElements.ServiceLevel)
            .Include(x => x.NonPriceElements.IntegrationTypes)
            .Include(x => x.NonPriceElements.Implementation)
            .Include(x => x.NonPriceElements.Features)
            .Include(x => x.NonPriceElements.NonPriceWeights)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Scores)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        competition.NonPriceElements.RemoveNonPriceElement(nonPriceElement);
        competition.NonPriceElements.NonPriceWeights = null;

        if (!competition.NonPriceElements.HasAnyNonPriceElements())
            competition.NonPriceElements = null;

        await dbContext.SaveChangesAsync();
    }

    public async Task AddFeatureRequirement(
        string internalOrgId,
        int competitionId,
        string requirements,
        CompliancyLevel compliance)
    {
        var competition = await dbContext.Competitions.Include(x => x.NonPriceElements.Features)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var nonPriceElements = competition.NonPriceElements ??= new NonPriceElements();
        nonPriceElements.Features.Add(new(requirements, compliance));

        await dbContext.SaveChangesAsync();
    }

    public async Task EditFeatureRequirement(
        string internalOrgId,
        int competitionId,
        int requirementId,
        string requirements,
        CompliancyLevel compliance)
    {
        var competition = await dbContext.Competitions.Include(x => x.NonPriceElements.Features)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var featureCriteria = competition.NonPriceElements.Features.FirstOrDefault(x => x.Id == requirementId);
        if (featureCriteria is null) return;

        featureCriteria.Requirements = requirements;
        featureCriteria.Compliance = compliance;

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteFeatureRequirement(string internalOrgId, int competitionId, int requirementId)
    {
        var competition = await dbContext.Competitions.Include(x => x.NonPriceElements.Features)
            .Include(x => x.NonPriceElements.ServiceLevel)
            .Include(x => x.NonPriceElements.IntegrationTypes)
            .Include(x => x.NonPriceElements.Implementation)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var featureCriteria = competition.NonPriceElements.Features.FirstOrDefault(x => x.Id == requirementId);
        if (featureCriteria is null) return;

        competition.NonPriceElements.Features.Remove(featureCriteria);

        if (!competition.NonPriceElements.HasAnyNonPriceElements())
            competition.NonPriceElements = null;

        await dbContext.SaveChangesAsync();
    }
}
