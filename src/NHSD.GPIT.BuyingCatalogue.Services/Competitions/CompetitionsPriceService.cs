using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;

namespace NHSD.GPIT.BuyingCatalogue.Services.Competitions;

public class CompetitionsPriceService : ICompetitionsPriceService
{
    private readonly BuyingCatalogueDbContext dbContext;
    private readonly ICompetitionsQuantityService quantityService;

    public CompetitionsPriceService(
        BuyingCatalogueDbContext dbContext,
        ICompetitionsQuantityService quantityService)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        this.quantityService = quantityService ?? throw new ArgumentNullException(nameof(quantityService));
    }

    public async Task SetSolutionPrice(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CataloguePrice cataloguePrice,
        IEnumerable<PricingTierDto> agreedPrices)
    {
        ArgumentNullException.ThrowIfNull(agreedPrices);

        var competitionSolution = await GetSolution(internalOrgId, competitionId, solutionId);
        if (competitionSolution == null)
            return;

        await SetPrice(
            competitionId,
            competitionSolution,
            competitionSolution.Price?.CataloguePriceId,
            cataloguePrice,
            () => quantityService.ResetSolutionQuantities(internalOrgId, competitionId, solutionId),
            agreedPrices);
    }

    public async Task SetServicePrice(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        CataloguePrice cataloguePrice,
        IEnumerable<PricingTierDto> agreedPrices)
    {
        ArgumentNullException.ThrowIfNull(agreedPrices);

        var service = await GetSolutionService(internalOrgId, competitionId, solutionId, serviceId);
        if (service == null) return;

        await SetPrice(
            competitionId,
            service,
            service.Price?.CataloguePriceId,
            cataloguePrice,
            () => quantityService.ResetServiceQuantities(internalOrgId, competitionId, solutionId, serviceId),
            agreedPrices);
    }

    private async Task SetPrice(
        int competitionId,
        ICompetitionPriceEntity entity,
        int? cataloguePriceId,
        CataloguePrice cataloguePrice,
        Func<Task> resetDelegate,
        IEnumerable<PricingTierDto> agreedPrices)
    {
        if (entity.Price != null)
        {
            var existingPrice = await dbContext.CataloguePrices
                .FirstAsync(x => x.CataloguePriceId == cataloguePriceId);

            if (existingPrice.HasDifferentQuantityBasisThan(cataloguePrice))
            {
                await resetDelegate();
            }

            dbContext.Remove(entity.Price);
        }

        entity.Price = new(cataloguePrice, competitionId);

        foreach (var agreedPrice in agreedPrices)
        {
            var tier = entity.Price
                .Tiers
                .First(x => x.LowerRange == agreedPrice.LowerRange
                    && x.UpperRange == agreedPrice.UpperRange);

            tier.Price = agreedPrice.Price;
        }

        await dbContext.SaveChangesAsync();
    }

    private async Task<CompetitionSolution> GetSolution(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId)
    {
        var competition = await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Price)
            .ThenInclude(x => x.Tiers)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var solution = competition.CompetitionSolutions.FirstOrDefault(x => x.SolutionId == solutionId);

        return solution;
    }

    private async Task<SolutionService> GetSolutionService(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId)
    {
        var competition = await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .ThenInclude(x => x.Price)
            .ThenInclude(x => x.Tiers)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var solution = competition.CompetitionSolutions.FirstOrDefault(x => x.SolutionId == solutionId);

        var service = solution?.SolutionServices.FirstOrDefault(x => x.ServiceId == serviceId);

        return service;
    }
}
