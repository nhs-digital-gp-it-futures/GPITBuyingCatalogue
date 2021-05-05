using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users
{
    public interface IUsersService
    {
        Task<AspNetUser> GetUser(string userId);
    }
}
