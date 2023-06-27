using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Models.Competitions;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public interface ICompetitionsService
{
    Task<int> AddCompetition(int organisationId, int filterId, string name, string description);

    Task<bool> Exists(int organisationId, string competitionName);

    Task<Competition> GetCompetition(int organisationId, int competitionId);

    Task<string> GetCompetitionName(int organisationId, int competitionId);

    Task<IEnumerable<Competition>> GetCompetitionsDashboard(int organisationId);

    Task<Competition> GetCompetitionWithWeightings(int organisationId, int competitionId);

    Task<Competition> GetCompetitionWithRecipients(int organisationId, int competitionId);

    Task<Competition> GetCompetitionWithServices(int organisationId, int competitionId, bool shouldTrack = false);

    Task<CompetitionTaskListModel> GetCompetitionTaskList(int organisationId, int competitionId);

    Task AddCompetitionSolutions(int organisationId, int competitionId, IEnumerable<CompetitionSolution> competitionSolutions);

    Task AcceptShortlist(int organisationId, int competitionId);

    Task CompleteCompetition(int organisationId, int competitionId);

    Task DeleteCompetition(int organisationId, int competitionId);

    Task SetCompetitionRecipients(int competitionId, IEnumerable<string> odsCodes);

    Task SetContractLength(int organisationId, int competitionId, int contractLength);

    Task SetCompetitionCriteria(int organisationId, int competitionId, bool includesNonPrice);

    Task SetCompetitionWeightings(int organisationId, int competitionId, int priceWeighting, int nonPriceWeighting);

    Task SetShortlistedSolutions(
        int organisationId,
        int competitionId,
        IEnumerable<CatalogueItemId> shortlistedSolutions);

    Task SetSolutionJustifications(
        int organisationId,
        int competitionId,
        Dictionary<CatalogueItemId, string> solutionsJustification);
}
