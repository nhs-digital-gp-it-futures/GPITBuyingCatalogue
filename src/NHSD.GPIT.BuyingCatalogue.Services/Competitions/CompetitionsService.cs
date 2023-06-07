using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Competitions;

public class CompetitionsService : ICompetitionsService
{
    private readonly BuyingCatalogueDbContext dbContext;

    public CompetitionsService(
        BuyingCatalogueDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IEnumerable<Competition>> GetCompetitions(int organisationId)
        => await dbContext.Competitions.Where(x => x.OrganisationId == organisationId)
            .ToListAsync();

    public async Task<Competition> GetCompetition(int organisationId, int competitionId)
        => await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Solution)
            .ThenInclude(x => x.CatalogueItem)
            .ThenInclude(x => x.Supplier)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.RequiredServices)
            .ThenInclude(x => x.Service)
            .ThenInclude(x => x.CatalogueItem)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.Id == competitionId);

    public async Task AddCompetitionSolutions(
        int organisationId,
        int competitionId,
        IEnumerable<CompetitionSolution> competitionSolutions)
    {
        var competition = await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.RequiredServices)
            .FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.Id == competitionId);

        competition.CompetitionSolutions.AddRange(competitionSolutions);

        await dbContext.SaveChangesAsync();
    }

    public async Task SetShortlistedSolutions(int organisationId, int competitionId, IEnumerable<CatalogueItemId> shortlistedSolutions)
    {
        var competition = await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.Id == competitionId);

        if (competition == null) return;

        var solutions = competition.CompetitionSolutions.ToList();

        solutions.ForEach(x => x.IsShortlisted = false);

        foreach (var competitionSolution in solutions.Where(x => shortlistedSolutions.Contains(x.SolutionId)))
        {
            competitionSolution.IsShortlisted = true;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task CompleteCompetition(int organisationId, int competitionId)
    {
        var competition =
            await dbContext.Competitions.FirstOrDefaultAsync(
                x => x.OrganisationId == organisationId && x.Id == competitionId);

        competition.Completed = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteCompetition(int organisationId, int competitionId)
    {
        var competition =
            await dbContext.Competitions.FirstOrDefaultAsync(
                x => x.OrganisationId == organisationId && x.Id == competitionId);

        competition.IsDeleted = true;

        await dbContext.SaveChangesAsync();
    }

    public async Task<int> AddCompetition(int organisationId, int filterId, string name, string description)
    {
        var competition = new Competition
        {
            OrganisationId = organisationId, FilterId = filterId, Name = name, Description = description,
        };

        dbContext.Competitions.Add(competition);

        await dbContext.SaveChangesAsync();

        return competition.Id;
    }

    public async Task<bool> ExistsAsync(int organisationId, string competitionName) =>
        await dbContext.Competitions.AnyAsync(
            x => x.OrganisationId == organisationId && string.Equals(x.Name, competitionName));
}
