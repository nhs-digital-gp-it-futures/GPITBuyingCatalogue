using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public interface ICompetitionsService
{
    Task<IEnumerable<Competition>> GetCompetitions(int organisationId);

    Task<Competition> GetCompetition(int organisationId, int competitionId);

    Task AddCompetitionSolutions(int organisationId, int competitionId, IEnumerable<CompetitionSolution> competitionSolutions);

    Task SetShortlistedSolutions(
        int organisationId,
        int competitionId,
        IEnumerable<CatalogueItemId> shortlistedSolutions);

    Task SetSolutionJustifications(
        int organisationId,
        int competitionId,
        Dictionary<CatalogueItemId, string> solutionsJustification);

    Task CompleteCompetition(int organisationId, int competitionId);

    Task DeleteCompetition(int organisationId, int competitionId);

    Task<int> AddCompetition(int organisationId, int filterId, string name, string description);

    Task<bool> ExistsAsync(int organisationId, string competitionName);
}
