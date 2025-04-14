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

        Task<int> GetCountForCompetitionSublocationRecipients(
            string externalOrgId,
            int competitionId,
            string sublocationOdsCode);

        Task AddSublocationRecipients(
            string parentOdsCode,
            int competitionId,
            string sublocationOdsCode,
            HashSet<string> recipientOdsCodes);

        Task RemoveSublocationRecipients(
            string parentOdsCode,
            int competitionId,
            string sublocationOdsCode,
            HashSet<string> recipientOdsCodes);

        Task SetSublocationRecipients(
            string parentOdsCode,
            int competitionId,
            string sublocationOdsCode,
            HashSet<string> newRecipientOdsCodes);
    }
}
