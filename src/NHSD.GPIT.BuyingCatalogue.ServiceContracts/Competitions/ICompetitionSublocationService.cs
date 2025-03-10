using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Competitions.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions
{
    public interface ICompetitionSublocationService
    {
        Task<CompetitionSublocation> GetCompetitionSublocationWithRecipients(string internalOrgId, int competitionId);
    }
}
