using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Enums;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users
{
    public interface ICreateUserService
    {
        Task<AspNetUser> Create(
            int primaryOrganisationId,
            string firstName,
            string lastName,
            string phoneNumber,
            string emailAddress,
            string organisationFunction,
            AccountStatus accountStatus);
    }
}
