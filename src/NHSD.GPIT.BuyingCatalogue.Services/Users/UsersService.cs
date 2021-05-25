using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.Identity;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Logging;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;

namespace NHSD.GPIT.BuyingCatalogue.Services.Users
{
    public class UsersService : IUsersService
    {
        private readonly ILogWrapper<UsersService> logger;
        private readonly IDbRepository<AspNetUser, UsersDbContext> userRepository;

        public UsersService(
            ILogWrapper<UsersService> logger,
            IDbRepository<AspNetUser, UsersDbContext> userRepository)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public async Task<AspNetUser> GetUser(string userId)
        {
            userId.ValidateNotNullOrWhiteSpace(nameof(userId));
            return await userRepository.SingleAsync(x => x.Id == userId);
        }

        public async Task<List<AspNetUser>> GetAllUsersForOrganisation(Guid organisationId)
        {
            organisationId.ValidateGuid(nameof(organisationId));
            return (await userRepository.GetAllAsync(x => x.PrimaryOrganisationId == organisationId)).ToList();
        }

        public async Task EnableOrDisableUser(string userId, bool disabled)
        {
            userId.ValidateNotNullOrWhiteSpace(nameof(userId));
            var user = await userRepository.SingleAsync(x => x.Id == userId);
            user.Disabled = disabled;
            await userRepository.SaveChangesAsync();
        }
    }
}
