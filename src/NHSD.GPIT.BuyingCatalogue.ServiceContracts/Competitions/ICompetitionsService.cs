using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public interface ICompetitionsService
{
    Task<IEnumerable<Competition>> GetCompetitions(int organisationId);

    Task AddCompetition(int organisationId, int filterId, string name, string description);

    Task<bool> ExistsAsync(int organisationId, string competitionName);
}
