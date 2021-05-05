using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHSD.GPIT.BuyingCatalogue.Services.Users
{
    public class UsersService : IUsersService
    {
        private readonly ILogWrapper<UsersService> _logger;
        private readonly IUsersDbRepository<AspNetUser> _userRepository;

        public UsersService(ILogWrapper<UsersService> logger,
            IUsersDbRepository<AspNetUser> userRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<AspNetUser> GetUser(string userId)
        {
            return await _userRepository.SingleAsync(x => x.Id == userId);
        }
    }
}
