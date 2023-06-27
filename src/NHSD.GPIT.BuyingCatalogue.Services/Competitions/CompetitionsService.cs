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
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.Competitions;

namespace NHSD.GPIT.BuyingCatalogue.Services.Competitions;

public class CompetitionsService : ICompetitionsService
{
    private readonly BuyingCatalogueDbContext dbContext;

    public CompetitionsService(
        BuyingCatalogueDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<IEnumerable<Competition>> GetCompetitionsDashboard(int organisationId)
        => await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .Where(x => x.OrganisationId == organisationId)
            .ToListAsync();

    public async Task<Competition> GetCompetitionWithWeightings(int organisationId, int competitionId) =>
        await dbContext.Competitions.Include(x => x.Weightings)
            .FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.Id == competitionId);

    public async Task<Competition> GetCompetitionWithServices(
        int organisationId,
        int competitionId,
        bool shouldTrack = false)
    {
        var query = dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Solution)
            .ThenInclude(x => x.CatalogueItem)
            .ThenInclude(x => x.Supplier)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.RequiredServices)
            .ThenInclude(x => x.Service)
            .ThenInclude(x => x.CatalogueItem)
            .AsSplitQuery();

        if (!shouldTrack)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.Id == competitionId);
    }

    public async Task<Competition> GetCompetition(int organisationId, int competitionId)
        => await dbContext.Competitions.AsNoTracking()
            .FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.Id == competitionId);

    public async Task<Competition> GetCompetitionWithRecipients(int organisationId, int competitionId)
        => await dbContext.Competitions.Include(x => x.Recipients)
            .AsNoTracking()
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

    public async Task SetContractLength(int organisationId, int competitionId, int contractLength)
    {
        var competition =
            await dbContext.Competitions.FirstOrDefaultAsync(
                x => x.OrganisationId == organisationId && x.Id == competitionId);

        competition.ContractLength = contractLength;

        await dbContext.SaveChangesAsync();
    }

    public async Task SetCompetitionCriteria(int organisationId, int competitionId, bool includesNonPrice)
    {
        var competition =
            await dbContext.Competitions.FirstOrDefaultAsync(
                x => x.OrganisationId == organisationId && x.Id == competitionId);

        competition.IncludesNonPrice = includesNonPrice;

        await dbContext.SaveChangesAsync();
    }

    public async Task SetCompetitionWeightings(int organisationId, int competitionId, int priceWeighting, int nonPriceWeighting)
    {
        var competition = await dbContext.Competitions.Include(x => x.Weightings)
            .FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.Id == competitionId);

        competition.Weightings ??= new Weightings();

        competition.Weightings.Price = priceWeighting;
        competition.Weightings.NonPrice = nonPriceWeighting;

        await dbContext.SaveChangesAsync();
    }

    public async Task SetShortlistedSolutions(
        int organisationId,
        int competitionId,
        IEnumerable<CatalogueItemId> shortlistedSolutions)
    {
        var competition = await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.Id == competitionId);

        if (competition == null) return;

        var solutions = competition.CompetitionSolutions.ToList();

        solutions.ForEach(x => x.IsShortlisted = false);
        foreach (var competitionSolution in solutions.Where(x => shortlistedSolutions.Contains(x.SolutionId)))
        {
            competitionSolution.Justification = null;
            competitionSolution.IsShortlisted = true;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task SetSolutionJustifications(
        int organisationId,
        int competitionId,
        Dictionary<CatalogueItemId, string> solutionsJustification)
    {
        if (solutionsJustification == null || solutionsJustification.Count == 0)
            return;

        var competition = await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.Id == competitionId);

        var solutions = competition.CompetitionSolutions.ToList();
        solutions.ForEach(x => x.Justification = null);

        foreach (var solution in solutions.Where(x => solutionsJustification.ContainsKey(x.SolutionId)))
        {
            solution.Justification = solutionsJustification[solution.SolutionId];
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task AcceptShortlist(int organisationId, int competitionId)
    {
        var competition =
            await dbContext.Competitions.FirstOrDefaultAsync(
                x => x.OrganisationId == organisationId && x.Id == competitionId);

        if (competition.ShortlistAccepted.HasValue)
            return;

        competition.ShortlistAccepted = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public async Task CompleteCompetition(int organisationId, int competitionId)
    {
        var competition =
            await dbContext.Competitions.FirstOrDefaultAsync(
                x => x.OrganisationId == organisationId && x.Id == competitionId);

        if (competition.Completed.HasValue)
            return;

        competition.Completed = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteCompetition(int organisationId, int competitionId)
    {
        var competition =
            await dbContext.Competitions.FirstOrDefaultAsync(
                x => x.OrganisationId == organisationId && x.Id == competitionId);

        if (competition == null)
            return;

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

    public async Task<bool> Exists(int organisationId, string competitionName) =>
        await dbContext.Competitions.AnyAsync(
            x => x.OrganisationId == organisationId && string.Equals(x.Name, competitionName));

    public async Task SetCompetitionRecipients(int competitionId, IEnumerable<string> odsCodes)
    {
        var recipients =
            await dbContext.CompetitionRecipients
                .Where(
                    x => x.CompetitionId == competitionId)
                .ToListAsync();

        var staleRecipients = recipients.Where(x => !odsCodes.Contains(x.OdsCode)).ToList();
        var newRecipients = odsCodes.Where(x => recipients.All(y => x != y.OdsCode)).ToList();

        dbContext.CompetitionRecipients.RemoveRange(staleRecipients);
        dbContext.CompetitionRecipients.AddRange(newRecipients.Select(x => new CompetitionRecipient(competitionId, x)));

        await dbContext.SaveChangesAsync();
    }

    public async Task<CompetitionTaskListModel> GetCompetitionTaskList(int organisationId, int competitionId) =>
        await dbContext
            .Competitions
            .Include(x => x.Weightings)
            .Include(x => x.Recipients)
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.OrganisationId == organisationId && x.Id == competitionId)
            .Select(
                x => new CompetitionTaskListModel(x))
            .FirstOrDefaultAsync();

    public async Task<string> GetCompetitionName(int organisationId, int competitionId) => await dbContext.Competitions
        .AsNoTracking()
        .Where(x => x.OrganisationId == organisationId && x.Id == competitionId)
        .Select(x => x.Name)
        .FirstOrDefaultAsync();
}
