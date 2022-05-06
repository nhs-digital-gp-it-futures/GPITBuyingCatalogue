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

        Task EnableOrDisableUser(int userId, bool disabled);

        Task UpdateUserAccountType(int userId, string organisationFunction);

        Task UpdateUserDetails(int userId, string firstName, string lastName, string email);

        Task UpdateUserOrganisation(int userId, int organisationId);

        Task<bool> EmailAddressExists(string emailAddress, int userId = 0);
    }
}
