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

    public CompetitionsPriceService(
        BuyingCatalogueDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task SetSolutionPrice(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CataloguePrice cataloguePrice,
        IEnumerable<PricingTierDto> agreedPrices)
    {
        var competition = await dbContext.Competitions
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Price)
            .ThenInclude(x => x.Tiers)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var competitionSolution = competition.CompetitionSolutions.FirstOrDefault(x => x.SolutionId == solutionId);
        if (competitionSolution == null)
            return;

        if (competitionSolution.Price != null)
        {
            var existingPrice = await dbContext.CataloguePrices
                .FirstAsync(x => x.CataloguePriceId == competitionSolution.Price.CataloguePriceId);

            if (existingPrice.HasDifferentQuantityBasisThan(cataloguePrice))
            {
                // TODO: Implement
            }

            dbContext.Remove(competitionSolution.Price);
        }

        competitionSolution.Price = new(cataloguePrice, competitionId);

        foreach (var agreedPrice in agreedPrices)
        {
            var tier = competitionSolution.Price
                .Tiers
                .First(x => x.LowerRange == agreedPrice.LowerRange
                    && x.UpperRange == agreedPrice.UpperRange);

            tier.Price = agreedPrice.Price;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task SetServicePrice(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        CatalogueItemId serviceId,
        CataloguePrice cataloguePrice,
        IEnumerable<PricingTierDto> agreedPrices)
    {
        var competition = await dbContext.Competitions
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .ThenInclude(x => x.Price)
            .ThenInclude(x => x.Tiers)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var competitionSolution = competition.CompetitionSolutions.FirstOrDefault(x => x.SolutionId == solutionId);

        var service = competitionSolution?.SolutionServices.FirstOrDefault(x => x.ServiceId == serviceId);
        if (service == null) return;

        if (service.Price != null)
        {
            var existingPrice = await dbContext.CataloguePrices
                .FirstAsync(x => x.CataloguePriceId == service.Price.CataloguePriceId);

            if (existingPrice.HasDifferentQuantityBasisThan(cataloguePrice))
            {
                // TODO: Implement
            }

            dbContext.Remove(service.Price);
        }

        service.Price = new(cataloguePrice, competitionId);

        foreach (var agreedPrice in agreedPrices)
        {
            var tier = service.Price
                .Tiers
                .First(x => x.LowerRange == agreedPrice.LowerRange
                    && x.UpperRange == agreedPrice.UpperRange);

            tier.Price = agreedPrice.Price;
        }

        await dbContext.SaveChangesAsync();
    }
}
