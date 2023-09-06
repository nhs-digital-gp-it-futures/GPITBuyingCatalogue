using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
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

    public async Task<Competition> GetCompetitionCriteriaReview(string internalOrgId, int competitionId) =>
        await dbContext.Competitions
            .Include(x => x.Organisation)
            .Include(x => x.Weightings)
            .Include(x => x.NonPriceElements)
            .Include(x => x.NonPriceElements.NonPriceWeights)
            .Include(x => x.NonPriceElements.Implementation)
            .Include(x => x.NonPriceElements.Interoperability)
            .Include(x => x.NonPriceElements.ServiceLevel)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

    public async Task<IEnumerable<Competition>> GetCompetitionsDashboard(string internalOrgId)
        => await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .Where(x => x.Organisation.InternalIdentifier == internalOrgId)
            .ToListAsync();

    public async Task<Competition> GetCompetitionForResults(string internalOrgId, int competitionId) =>
        await dbContext
            .Competitions
            .Include(x => x.Weightings)
            .Include(x => x.Recipients)
            .Include(x => x.NonPriceElements)
            .Include(x => x.NonPriceElements.NonPriceWeights)
            .Include(x => x.NonPriceElements.Implementation)
            .Include(x => x.NonPriceElements.Interoperability)
            .Include(x => x.NonPriceElements.ServiceLevel)
            .Include(x => x.CompetitionSolutions).ThenInclude(x => x.Scores)
            .Include(x => x.CompetitionSolutions).ThenInclude(x => x.Price).ThenInclude(x => x.Tiers)
            .Include(x => x.CompetitionSolutions).ThenInclude(x => x.Quantities)
            .Include(x => x.CompetitionSolutions).ThenInclude(x => x.Solution.CatalogueItem.Supplier)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices).ThenInclude(x => x.Quantities)
            .Include(x => x.CompetitionSolutions).ThenInclude(x => x.SolutionServices).ThenInclude(x => x.Price).ThenInclude(x => x.Tiers)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

    public async Task<Competition> GetCompetitionWithNonPriceElements(string internalOrgId, int competitionId)
        => await dbContext.Competitions
            .Include(x => x.Organisation)
            .Include(x => x.NonPriceElements.Implementation)
            .Include(x => x.NonPriceElements.Interoperability)
            .Include(x => x.NonPriceElements.ServiceLevel)
            .Include(x => x.NonPriceElements.NonPriceWeights)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Scores)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

    public async Task<Competition> GetCompetitionWithWeightings(string internalOrgId, int competitionId) =>
        await dbContext.Competitions.Include(x => x.Weightings)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

    public async Task<Competition> GetCompetitionWithServices(
        string internalOrgId,
        int competitionId,
        bool shouldTrack = false)
    {
        var query = dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Solution)
            .ThenInclude(x => x.CatalogueItem)
            .ThenInclude(x => x.Supplier)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .ThenInclude(x => x.Service)
            .IgnoreQueryFilters()
            .AsSplitQuery();

        if (!shouldTrack)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);
    }

    public async Task<Competition> GetCompetitionWithSolutions(string internalOrgId, int competitionId)
        => await dbContext.Competitions
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Solution)
            .ThenInclude(x => x.CatalogueItem)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Solution.ServiceLevelAgreement.ServiceHours)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Scores)
            .Include(x => x.NonPriceElements.Interoperability)
            .Include(x => x.NonPriceElements.Implementation)
            .Include(x => x.NonPriceElements.ServiceLevel)
            .Include(x => x.Recipients)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

    public async Task<Competition> GetCompetitionWithSolutionsHub(string internalOrgId, int competitionId)
        => await dbContext.Competitions
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Solution)
            .ThenInclude(x => x.CatalogueItem)
            .ThenInclude(x => x.CataloguePrices)
            .ThenInclude(x => x.CataloguePriceTiers)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Price)
            .ThenInclude(x => x.Tiers)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .ThenInclude(x => x.Price)
            .ThenInclude(x => x.Tiers)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .ThenInclude(x => x.Service)
            .ThenInclude(x => x.CataloguePrices)
            .ThenInclude(x => x.CataloguePriceTiers)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .ThenInclude(x => x.Quantities)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Quantities)
            .Include(x => x.Recipients)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

    public async Task<Competition> GetCompetition(string internalOrgId, int competitionId)
        => await dbContext.Competitions.AsNoTracking()
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

    public async Task<Competition> GetCompetitionWithRecipients(string internalOrgId, int competitionId)
        => await dbContext.Competitions.Include(x => x.Recipients)
            .AsNoTracking()
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

    public async Task AddCompetitionSolutions(
        string internalOrgId,
        int competitionId,
        IEnumerable<CompetitionSolution> competitionSolutions)
    {
        var competition = await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        competition.CompetitionSolutions.AddRange(competitionSolutions);

        await dbContext.SaveChangesAsync();
    }

    public async Task SetContractLength(string internalOrgId, int competitionId, int contractLength)
    {
        var competition =
            await dbContext.Competitions.FirstOrDefaultAsync(
                x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        competition.ContractLength = contractLength;

        await dbContext.SaveChangesAsync();
    }

    public async Task SetCompetitionCriteria(string internalOrgId, int competitionId, bool includesNonPrice)
    {
        var competition =
            await dbContext.Competitions.FirstOrDefaultAsync(
                x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        competition.IncludesNonPrice = includesNonPrice;

        await dbContext.SaveChangesAsync();
    }

    public async Task SetCompetitionWeightings(string internalOrgId, int competitionId, int priceWeighting, int nonPriceWeighting)
    {
        var competition = await dbContext.Competitions.Include(x => x.Weightings)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        competition.Weightings ??= new Weightings();

        competition.Weightings.Price = priceWeighting;
        competition.Weightings.NonPrice = nonPriceWeighting;
        competition.HasReviewedCriteria = false;

        await dbContext.SaveChangesAsync();
    }

    public async Task SetCriteriaReviewed(string internalOrgId, int competitionId)
    {
        var competition = await dbContext.Competitions.FirstOrDefaultAsync(
            x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        competition.HasReviewedCriteria = true;

        await dbContext.SaveChangesAsync();
    }

    public async Task SetImplementationCriteria(string internalOrgId, int competitionId, string requirements)
    {
        var competition = await dbContext.Competitions.Include(x => x.NonPriceElements.Implementation)
            .Include(x => x.CompetitionSolutions).ThenInclude(x => x.Scores)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var nonPriceElements = competition.NonPriceElements ??= new NonPriceElements();
        var implementation = nonPriceElements.Implementation ??= new ImplementationCriteria();

        implementation.Requirements = requirements;

        if (dbContext.Entry(implementation).State is EntityState.Modified)
        {
            competition.HasReviewedCriteria = false;
            RemoveScoreType(ScoreType.Implementation, competition);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task SetInteroperabilityCriteria(
        string internalOrgId,
        int competitionId,
        IEnumerable<string> im1Integrations,
        IEnumerable<string> gpConnectIntegrations)
    {
        var competition = await dbContext.Competitions.Include(x => x.NonPriceElements.Interoperability)
            .Include(x => x.CompetitionSolutions).ThenInclude(x => x.Scores)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var interopEntities = (competition.NonPriceElements ??= new NonPriceElements()).Interoperability;

        var staleEntities = interopEntities.Where(
            x => !im1Integrations.Contains(x.Qualifier) && !gpConnectIntegrations.Contains(x.Qualifier))
            .ToList();

        if (staleEntities.Any()) staleEntities.ForEach(x => interopEntities.Remove(x));

        var newInteropEntities = im1Integrations
            .Select(x => new InteroperabilityCriteria(x, InteropIntegrationType.Im1))
            .Union(
                gpConnectIntegrations.Select(x => new InteroperabilityCriteria(x, InteropIntegrationType.GpConnect)))
            .Where(x => interopEntities.All(y => x.Qualifier != y.Qualifier))
            .ToList();

        interopEntities.AddRange(newInteropEntities);
        if (staleEntities.Any() || newInteropEntities.Any())
        {
            competition.HasReviewedCriteria = false;
            RemoveScoreType(ScoreType.Interoperability, competition);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task SetNonPriceWeights(
        string internalOrgId,
        int competitionId,
        int? implementationWeight,
        int? interoperabilityWeight,
        int? serviceLevelWeight)
    {
        var competition = await dbContext.Competitions.Include(x => x.NonPriceElements.NonPriceWeights)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var nonPriceWeights = competition.NonPriceElements.NonPriceWeights ??= new NonPriceWeights();

        nonPriceWeights.Implementation = implementationWeight;
        nonPriceWeights.Interoperability = interoperabilityWeight;
        nonPriceWeights.ServiceLevel = serviceLevelWeight;

        if (dbContext.Entry(nonPriceWeights).State is EntityState.Modified)
            competition.HasReviewedCriteria = false;

        await dbContext.SaveChangesAsync();
    }

    public async Task SetServiceLevelCriteria(
        string internalOrgId,
        int competitionId,
        DateTime timeFrom,
        DateTime timeUntil,
        string applicableDays)
    {
        var competition = await dbContext.Competitions.Include(x => x.NonPriceElements.ServiceLevel)
            .Include(x => x.CompetitionSolutions).ThenInclude(x => x.Scores)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var nonPriceElements = competition.NonPriceElements ??= new NonPriceElements();
        var serviceLevelCriteria = nonPriceElements.ServiceLevel ??= new ServiceLevelCriteria();

        serviceLevelCriteria.TimeFrom = timeFrom;
        serviceLevelCriteria.TimeUntil = timeUntil;
        serviceLevelCriteria.ApplicableDays = applicableDays;
        if (dbContext.Entry(serviceLevelCriteria).State is EntityState.Modified)
        {
            competition.HasReviewedCriteria = false;
            RemoveScoreType(ScoreType.ServiceLevel, competition);
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task SetShortlistedSolutions(
        string internalOrgId,
        int competitionId,
        IEnumerable<CatalogueItemId> shortlistedSolutions)
    {
        var competition = await dbContext.Competitions.IgnoreQueryFilters()
            .Include(x => x.CompetitionSolutions)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

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
        string internalOrgId,
        int competitionId,
        Dictionary<CatalogueItemId, string> solutionsJustification)
    {
        if (solutionsJustification == null || solutionsJustification.Count == 0)
            return;

        var competition = await dbContext.Competitions.IgnoreQueryFilters()
            .Include(x => x.CompetitionSolutions)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var solutions = competition.CompetitionSolutions.ToList();
        solutions.ForEach(x => x.Justification = null);

        foreach (var solution in solutions.Where(x => solutionsJustification.ContainsKey(x.SolutionId)))
        {
            solution.Justification = solutionsJustification[solution.SolutionId];
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task SetSolutionsImplementationScores(
        string internalOrgId,
        int competitionId,
        Dictionary<CatalogueItemId, int> solutionsScores)
        => await SetSolutionScores(
            internalOrgId,
            competitionId,
            solutionsScores ?? throw new ArgumentNullException(nameof(solutionsScores)),
            ScoreType.Implementation);

    public async Task SetSolutionsInteroperabilityScores(
        string internalOrgId,
        int competitionId,
        Dictionary<CatalogueItemId, int> solutionsScores)
        => await SetSolutionScores(
            internalOrgId,
            competitionId,
            solutionsScores ?? throw new ArgumentNullException(nameof(solutionsScores)),
            ScoreType.Interoperability);

    public async Task SetSolutionsServiceLevelScores(
        string internalOrgId,
        int competitionId,
        Dictionary<CatalogueItemId, int> solutionsScores)
        => await SetSolutionScores(
            internalOrgId,
            competitionId,
            solutionsScores ?? throw new ArgumentNullException(nameof(solutionsScores)),
            ScoreType.ServiceLevel);

    public async Task AcceptShortlist(string internalOrgId, int competitionId)
    {
        var competition =
            await dbContext.Competitions.FirstOrDefaultAsync(
                x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        if (competition.ShortlistAccepted.HasValue)
            return;

        competition.ShortlistAccepted = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public async Task CompleteCompetition(string internalOrgId, int competitionId, bool isDirectAward = false)
    {
        var competition =
            await dbContext.Competitions
                .Include(x => x.Weightings)
                .Include(x => x.NonPriceElements.NonPriceWeights)
                .Include(x => x.NonPriceElements.Interoperability)
                .Include(x => x.NonPriceElements.Implementation)
                .Include(x => x.NonPriceElements.ServiceLevel)
                .Include(x => x.CompetitionSolutions).ThenInclude(x => x.Scores)
                .Include(x => x.CompetitionSolutions).ThenInclude(x => x.Quantities)
                .Include(x => x.CompetitionSolutions).ThenInclude(x => x.Price).ThenInclude(x => x.Tiers)
                .Include(x => x.CompetitionSolutions).ThenInclude(x => x.SolutionServices).ThenInclude(x => x.Quantities)
                .Include(x => x.CompetitionSolutions).ThenInclude(x => x.SolutionServices).ThenInclude(x => x.Price).ThenInclude(x => x.Tiers)
                .AsSplitQuery()
                .FirstOrDefaultAsync(
                x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        if (competition.Completed.HasValue)
            return;

        if (!isDirectAward)
        {
            ScoreSolutionPrices(competition);
            SetNonPriceScoreWeightings(competition);
            SetWinningSolution(competition);
        }

        competition.Completed = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteCompetition(string internalOrgId, int competitionId)
    {
        var competition =
            await dbContext.Competitions.FirstOrDefaultAsync(
                x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        if (competition == null)
            return;

        competition.IsDeleted = true;

        await dbContext.SaveChangesAsync();
    }

    public async Task RemoveNonPriceElements(string internalOrgId, int competitionId)
    {
        var competition = await dbContext.Competitions
            .Include(x => x.NonPriceElements)
            .Include(x => x.Weightings)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var nonPriceElementScores = ScoreTypeExtensions.GetNonPriceElementScores();
        var competitionSolutionScores = await dbContext.CompetitionSolutionScores
            .Where(x => x.CompetitionId == competitionId && nonPriceElementScores.Contains(x.ScoreType))
            .ToListAsync();

        competition.HasReviewedCriteria = false;

        if (competition.Weightings != null)
            dbContext.Remove(competition.Weightings);

        if (competition.NonPriceElements != null)
            dbContext.Remove(competition.NonPriceElements);

        if (competitionSolutionScores.Count > 0)
            dbContext.RemoveRange(competitionSolutionScores);

        if (dbContext.ChangeTracker.HasChanges())
            await dbContext.SaveChangesAsync();
    }

    public async Task SetAssociatedServices(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        IEnumerable<CatalogueItemId> associatedServices)
    {
        ArgumentNullException.ThrowIfNull(associatedServices);

        var competition = await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .ThenInclude(x => x.Service)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var solution = competition.CompetitionSolutions.FirstOrDefault(x => x.SolutionId == solutionId);
        if (solution == null) return;

        var existingAssociatedServices = solution.SolutionServices.Where(
            x => !x.IsRequired && x.Service.CatalogueItemType is CatalogueItemType.AssociatedService);

        var selectedAssociatedServices = associatedServices.ToList();

        var toAdd = selectedAssociatedServices.Where(x => existingAssociatedServices.All(y => y.ServiceId != x))
            .Select(x => new SolutionService(competitionId, solutionId, x, false))
            .ToList();

        var toRemove = existingAssociatedServices.Where(x => !selectedAssociatedServices.Contains(x.ServiceId))
            .ToList();

        var pricesToRemove = toRemove.Where(x => x.Price != null).Select(x => x.Price).ToList();
        if (pricesToRemove.Any())
            dbContext.RemoveRange(pricesToRemove);

        toRemove.ForEach(x => solution.SolutionServices.Remove(x));
        toAdd.ForEach(x => solution.SolutionServices.Add(x));

        if (dbContext.ChangeTracker.HasChanges())
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

    public async Task<bool> Exists(string internalOrgId, string competitionName) =>
        await dbContext.Competitions.AnyAsync(
            x => x.Organisation.InternalIdentifier == internalOrgId && string.Equals(x.Name, competitionName));

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

    public async Task<CompetitionTaskListModel> GetCompetitionTaskList(string internalOrgId, int competitionId) =>
        await dbContext
            .Competitions
            .Include(x => x.Weightings)
            .Include(x => x.Recipients)
            .Include(x => x.NonPriceElements)
            .Include(x => x.NonPriceElements.NonPriceWeights)
            .Include(x => x.NonPriceElements.Implementation)
            .Include(x => x.NonPriceElements.Interoperability)
            .Include(x => x.NonPriceElements.ServiceLevel)
            .Include(x => x.CompetitionSolutions).ThenInclude(x => x.Scores)
            .Include(x => x.CompetitionSolutions).ThenInclude(x => x.Price)
            .Include(x => x.CompetitionSolutions).ThenInclude(x => x.Quantities)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .ThenInclude(x => x.Quantities)
            .Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.SolutionServices)
            .ThenInclude(x => x.Price)
            .AsNoTracking()
            .AsSplitQuery()
            .Where(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId)
            .Select(
                x => new CompetitionTaskListModel(x))
            .FirstOrDefaultAsync();

    public async Task<string> GetCompetitionName(string internalOrgId, int competitionId) => await dbContext.Competitions
        .AsNoTracking()
        .Where(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId)
        .Select(x => x.Name)
        .FirstOrDefaultAsync();

    private static void RemoveScoreType(ScoreType scoreType, Competition competition)
    {
        foreach (var solution in competition.CompetitionSolutions.Where(x => x.HasScoreType(scoreType)))
        {
            var score = solution.GetScoreByType(scoreType);

            solution.Scores.Remove(score);
        }
    }

    private static void ScoreSolutionPrices(Competition competition)
    {
        static decimal GetWeightedPriceScore(Competition competition, int score)
        {
            var weight = competition.IncludesNonPrice.GetValueOrDefault()
                ? competition.Weightings.Price.GetValueOrDefault()
                : 100;

            return CompetitionFormulas.CalculateWeightedScore(
                score,
                weight);
        }

        var competitionSolutions = competition.CompetitionSolutions;
        var orderedSolutions = competitionSolutions
            .Select(x => (Solution: x, Price: x.CalculateTotalPrice(competition.ContractLength.GetValueOrDefault())!.Value))
            .OrderBy(x => x.Price)
            .ToList();

        const int maxScore = 5;

        var lowestPricedSolution = orderedSolutions.First();
        lowestPricedSolution.Solution.Scores.Add(new SolutionScore(ScoreType.Price, maxScore, GetWeightedPriceScore(competition, maxScore)));

        var remainingSolutions = orderedSolutions.Skip(1);

        foreach ((CompetitionSolution solution, var currentPrice) in remainingSolutions)
        {
            var score = CompetitionFormulas.CalculatePriceIncreaseScore(lowestPricedSolution.Price, currentPrice);

            var priceWeightedScore = GetWeightedPriceScore(competition, score);

            solution.Scores.Add(new SolutionScore(ScoreType.Price, score, priceWeightedScore));
        }
    }

    private static void SetNonPriceScoreWeightings(Competition competition)
    {
        if (!competition.IncludesNonPrice.GetValueOrDefault()) return;

        var competitionSolutions = competition.CompetitionSolutions;
        var selectedNonPriceElements = competition.NonPriceElements.GetNonPriceElements()
            .Select(
                x => (NonPriceElement: x,
                    NonPriceWeight: competition.NonPriceElements.GetNonPriceWeight(x).GetValueOrDefault()))
            .ToList();

        foreach (var competitionSolution in competitionSolutions)
        {
            foreach (var nonPriceElementAndWeight in selectedNonPriceElements)
            {
                var nonPriceElementScore = competitionSolution.GetScoreByType(nonPriceElementAndWeight.NonPriceElement.AsScoreType());

                var nonPriceElementWeightedScore = CompetitionFormulas.CalculateWeightedScore(
                    nonPriceElementScore.Score,
                    nonPriceElementAndWeight.NonPriceWeight);

                nonPriceElementScore.WeightedScore = nonPriceElementWeightedScore;
            }
        }
    }

    private static void SetWinningSolution(Competition competition)
    {
        var solutionsAndScores = new Dictionary<CompetitionSolution, decimal>();
        foreach (var competitionSolution in competition.CompetitionSolutions)
        {
            var priceScore = competitionSolution.GetScoreByType(ScoreType.Price);
            var nonPriceScores = competitionSolution.Scores.Where(x => x.ScoreType is not ScoreType.Price).ToList();

            var nonPriceScoreSum = nonPriceScores.Sum(x => x.WeightedScore);
            var nonPriceWeightedScore = CompetitionFormulas.CalculateWeightedScore(
                nonPriceScoreSum,
                competition.Weightings?.NonPrice.GetValueOrDefault() ?? 0);

            var totalScore = nonPriceWeightedScore + priceScore.Score;
            solutionsAndScores[competitionSolution] = totalScore;
        }

        var winningSolutions = solutionsAndScores
            .GroupBy(x => x.Value)
            .OrderByDescending(x => x.Key)
            .First()
            .Select(x => x.Key)
            .ToList();

        winningSolutions.ForEach(x => x.IsWinningSolution = true);
    }

    private async Task SetSolutionScores(
        string internalOrgId,
        int competitionId,
        Dictionary<CatalogueItemId, int> solutionsScores,
        ScoreType scoreType)
    {
        var competition = await dbContext.Competitions.Include(x => x.CompetitionSolutions)
            .ThenInclude(x => x.Scores)
            .FirstOrDefaultAsync(x => x.Organisation.InternalIdentifier == internalOrgId && x.Id == competitionId);

        var solutions = competition.CompetitionSolutions;

        foreach (var solutionAndScore in solutionsScores)
        {
            var solution = solutions.First(x => x.SolutionId == solutionAndScore.Key);
            SolutionScore solutionScore;

            if (solution.HasScoreType(scoreType))
            {
                solutionScore = solution.GetScoreByType(scoreType);
                solutionScore.Score = solutionAndScore.Value;

                continue;
            }

            solutionScore = new SolutionScore(scoreType, solutionAndScore.Value);
            solution.Scores.Add(solutionScore);
        }

        await dbContext.SaveChangesAsync();
    }
}
