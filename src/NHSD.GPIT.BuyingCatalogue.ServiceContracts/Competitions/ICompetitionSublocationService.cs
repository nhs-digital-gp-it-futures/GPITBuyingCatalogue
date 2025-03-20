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
            string sublocationOdsCode);

        Task AddSublocationRecipients(
            string parentOdsCode,
            int competitionId,
            string sublocationOdsCode,
            HashSet<string> recipientOdsCodes);

        Task RemoveSublocationRecipients(
            string externalOrgId,
            int competitionId,
            string sublocationOdsCode,
            HashSet<string> recipientOdsCodes);
    }
}
