using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.Competitions;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public interface ICompetitionsService
{
    Task<int> AddCompetition(int organisationId, int filterId, string name, string description);

    Task<bool> Exists(string internalOrgId, string competitionName);

    Task<Competition> GetCompetition(string internalOrgId, int competitionId);

    Task<string> GetCompetitionName(string internalOrgId, int competitionId);

    Task<Competition> GetCompetitionCriteriaReview(string internalOrgId, int competitionId);

    Task<List<Competition>> GetCompetitions(string internalOrgId);

    Task<PagedList<Competition>> GetPagedCompetitions(string internalOrgId, PageOptions options);

    Task<Competition> GetCompetitionForResults(string internalOrgId, int competitionId);

    Task<Competition> GetCompetitionWithNonPriceElements(string internalOrgId, int competitionId);

    Task<Competition> GetCompetitionWithWeightings(string internalOrgId, int competitionId);

    Task<Competition> GetCompetitionWithRecipients(string internalOrgId, int competitionId);

    Task<Competition> GetCompetitionWithServices(string internalOrgId, int competitionId, bool shouldTrack = false);

    Task<Competition> GetCompetitionWithSolutions(string internalOrgId, int competitionId);

    Task<Competition> GetCompetitionWithSolutionsHub(string internalOrgId, int competitionId);

    Task<CompetitionTaskListModel> GetCompetitionTaskList(string internalOrgId, int competitionId);

    Task<ICollection<CompetitionSolution>> GetNonShortlistedSolutions(string internalOrgId, int competitionId);

    Task AddCompetitionSolutions(string internalOrgId, int competitionId, IEnumerable<CompetitionSolution> competitionSolutions);

    Task AcceptShortlist(string internalOrgId, int competitionId);

    Task CompleteCompetition(string internalOrgId, int competitionId, bool isDirectAward = false);

    Task DeleteCompetition(string internalOrgId, int competitionId);

    Task RemoveNonPriceElements(string internalOrgId, int competitionId);

    Task SetAssociatedServices(
        string internalOrgId,
        int competitionId,
        CatalogueItemId solutionId,
        IEnumerable<CatalogueItemId> associatedServices);

    Task SetCompetitionRecipients(int competitionId, IEnumerable<string> odsCodes);

    Task SetContractLength(string internalOrgId, int competitionId, int contractLength);

    Task SetCompetitionCriteria(string internalOrgId, int competitionId, bool includesNonPrice);

    Task SetCompetitionWeightings(string internalOrgId, int competitionId, int priceWeighting, int nonPriceWeighting);

    Task SetCriteriaReviewed(string internalOrgId, int competitionId);

    Task SetImplementationCriteria(string internalOrgId, int competitionId, string requirements);

    Task SetInteroperabilityCriteria(
        string internalOrgId,
        int competitionId,
        IEnumerable<string> im1Integrations,
        IEnumerable<string> gpConnectIntegrations);

    Task SetNonPriceWeights(
        string internalOrgId,
        int competitionId,
        int? implementationWeight,
        int? interoperabilityWeight,
        int? serviceLevelWeight,
        int? featuresWeight);

    Task SetServiceLevelCriteria(
        string internalOrgId,
        int competitionId,
        DateTime timeFrom,
        DateTime timeUntil,
        IEnumerable<Iso8601DayOfWeek> applicableDays,
        bool includesBankHolidays);

    Task SetShortlistedSolutions(
        string internalOrgId,
        int competitionId,
        IEnumerable<CatalogueItemId> shortlistedSolutions);

    Task SetSolutionJustifications(
        string internalOrgId,
        int competitionId,
        Dictionary<CatalogueItemId, string> solutionsJustification);

    Task SetSolutionsImplementationScores(
        string internalOrgId,
        int competitionId,
        Dictionary<CatalogueItemId, (int Score, string Justification)> solutionsScores);

    Task SetSolutionsInteroperabilityScores(
        string internalOrgId,
        int competitionId,
        Dictionary<CatalogueItemId, (int Score, string Justification)> solutionsScores);

    Task SetSolutionsServiceLevelScores(
        string internalOrgId,
        int competitionId,
        Dictionary<CatalogueItemId, (int Score, string Justification)> solutionsScores);

    Task SetSolutionsFeaturesScores(
        string internalOrgId,
        int competitionId,
        Dictionary<CatalogueItemId, (int Score, string Justification)> solutionsScores);
}
