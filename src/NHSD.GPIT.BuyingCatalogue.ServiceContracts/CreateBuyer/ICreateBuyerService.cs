using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Results;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer
{
    public interface ICreateBuyerService
    {
        Task<Result<int>> Create(int primaryOrganisationId, string firstName, string lastName, string phoneNumber, string emailAddress);
    }
}
