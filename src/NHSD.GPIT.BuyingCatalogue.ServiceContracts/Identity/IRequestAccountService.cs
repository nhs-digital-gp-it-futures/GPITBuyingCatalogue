using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity
{
    public interface IRequestAccountService
    {
        Task RequestAccount(NewAccountDetails request);
    }
}
