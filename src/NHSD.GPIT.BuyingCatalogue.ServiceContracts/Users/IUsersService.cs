using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users
{
    public interface IUsersService
    {
        Task<AspNetUser> GetUser(int userId);

        Task<List<AspNetUser>> GetAllUsersForOrganisation(int organisationId);

        Task EnableOrDisableUser(int userId, bool disabled);
    }
}
