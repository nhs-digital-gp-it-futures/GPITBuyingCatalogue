using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Verification
{
    public interface IVerificationService
    {
        Task<bool> VerifyUrl(string uRL);
    }
}
