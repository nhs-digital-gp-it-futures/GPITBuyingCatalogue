using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions
{
    public interface ICompetitionSublocationService
    {
        Task<CompetitionSublocation> GetCompetitionSublocationWithRecipients(
            string externalOrgId,
            int competitionId,
            string sublocationId);

        Task AddSublocationRecipients(
            string externalOrgId,
            int competitionId,
            HashSet<string> sublocationIds);

        Task RemoveSublocationRecipients(
            string externalOrgId,
            int competitionId,
            HashSet<string> sublocationIds);
    }
}
