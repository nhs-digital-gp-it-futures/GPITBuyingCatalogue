using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;

namespace NHSD.GPIT.BuyingCatalogue.Services.Users
{
    public sealed class UsersService : IUsersService
    {
        private readonly IDbRepository<AspNetUser, BuyingCatalogueDbContext> userRepository;

        public UsersService(
            IDbRepository<AspNetUser, BuyingCatalogueDbContext> userRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public Task<AspNetUser> GetUser(int userId) => userRepository.SingleAsync(u => u.Id == userId);

        public async Task<List<AspNetUser>> GetAllUsersForOrganisation(int organisationId)
        {
            return (await userRepository.GetAllAsync(u => u.PrimaryOrganisationId == organisationId)).ToList();
        }

        public async Task EnableOrDisableUser(int userId, bool disabled)
        {
            var user = await userRepository.SingleAsync(u => u.Id == userId);
            user.Disabled = disabled;
            await userRepository.SaveChangesAsync();
        }
    }
}
