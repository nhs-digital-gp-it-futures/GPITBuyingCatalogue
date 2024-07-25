using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users
{
    public interface IUsersService
    {
        Task<AspNetUser> GetUser(int userId);

        Task<List<AspNetUser>> GetAllUsers();

        Task<List<AspNetUser>> GetAllUsersBySearchTerm(string searchTerm);

        Task<List<AspNetUser>> GetAllUsersForOrganisation(int organisationId);

        Task UpdateUser(
            int userId,
            string firstName,
            string lastName,
            string email,
            bool disabled,
            string organisationFunction,
            int organisationId);

        Task SetTermsOfUse(int userId, bool hasOptedInUserResearch);

        Task<bool> EmailAddressExists(string emailAddress, int userId = 0);

        Task<bool> IsAccountManagerLimit(int organisationId, int userId = 0);
    }
}
