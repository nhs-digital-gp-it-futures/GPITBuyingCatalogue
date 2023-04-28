using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public interface INominateOrganisationService
    {
        Task NominateOrganisation(int userId, NominateOrganisationRequest request);

        Task<bool> IsGpPractice(int userId);
    }
}
