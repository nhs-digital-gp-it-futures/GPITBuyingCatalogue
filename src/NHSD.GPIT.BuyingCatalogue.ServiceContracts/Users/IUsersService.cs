using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users
{
    public interface IUsersService
    {
        Task<AspNetUser> GetUser(Guid userId);

        Task<List<AspNetUser>> GetAllUsersForOrganisation(int organisationId);

        Task EnableOrDisableUser(Guid userId, bool disabled);
    }
}
