using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.CreateBuyer
{
    public interface ICreateBuyerService
    {
        Task<AspNetUser> Create(int primaryOrganisationId, string firstName, string lastName, string phoneNumber, string emailAddress, bool isAdmin);

        Task<bool> UserExistsWithEmail(string emailAddress);
    }
}
