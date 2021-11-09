using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Verification
{
    public interface IUrlVerificationService
    {
        Task<bool> VerifyUrl(string siteLink);
    }
}
