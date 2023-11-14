using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Security;

public interface IRecaptchaVerificationService
{
    Task<bool> Validate(string recaptchaResponse);
}
