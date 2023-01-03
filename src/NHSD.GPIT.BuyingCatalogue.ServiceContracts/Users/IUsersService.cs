using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users
{
    public interface IUsersService
    {
        Task<AspNetUser> GetUser(int userId);

        Task<List<AspNetUser>> GetAllUsers();

        Task<bool> HasRole(int userId, string role);

        Task<IList<string>> GetRoles(AspNetUser user);

        Task<List<AspNetUser>> GetAllUsersBySearchTerm(string searchTerm);

        Task<List<AspNetUser>> GetAllUsersForOrganisation(int organisationId);

        Task EnableOrDisableUser(int userId, bool disabled);

        Task UpdateUserAccountType(int userId, string organisationFunction);

        Task UpdateUserDetails(int userId, string firstName, string lastName, string email);

        Task UpdateUserOrganisation(int userId, int organisationId);

        Task UpdateUser(
            int userId,
            string firstName,
            string lastName,
            string email,
            bool disabled,
            string organisationFunction,
            int organisationId);

        Task<bool> EmailAddressExists(string emailAddress, int userId = 0);

        Task<bool> IsAccountManagerLimit(int organisationId, int userId = 0);

        bool IsPasswordPresentInPastNPasswords(AspNetUser user, string email, string newPassword);
    }
}
