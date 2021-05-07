using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Organisations
{
    public interface IOdsService
    {
        Task<OdsOrganisation> GetOrganisationByOdsCode(string odsCode);
    }
}
