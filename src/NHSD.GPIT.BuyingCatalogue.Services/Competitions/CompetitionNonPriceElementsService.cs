using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
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
            .Include(x => x.NonPriceElements.Interoperability)
            .Include(x => x.NonPriceElements.Implementation)
            .Include(x => x.NonPriceElements.NonPriceWeights)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Scores)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        competition.NonPriceElements.RemoveNonPriceElement(nonPriceElement);
        competition.NonPriceElements.NonPriceWeights = null;
        competition.HasReviewedCriteria = false;

        if (!competition.NonPriceElements.HasAnyNonPriceElements())
            competition.NonPriceElements = null;

        foreach (var solution in competition.CompetitionSolutions)
        {
            var nonPriceElementScores =
                solution.Scores.Where(x => x.ScoreType == nonPriceElement.AsScoreType()).ToList();

            nonPriceElementScores.ForEach(x => solution.Scores.Remove(x));
        }

        await dbContext.SaveChangesAsync();
    }
}
