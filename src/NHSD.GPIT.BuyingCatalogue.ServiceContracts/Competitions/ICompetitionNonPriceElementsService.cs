using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Competitions;

public interface ICompetitionNonPriceElementsService
{
    Task DeleteNonPriceElement(string internalOrgId, int competitionId, NonPriceElement nonPriceElement);
}
