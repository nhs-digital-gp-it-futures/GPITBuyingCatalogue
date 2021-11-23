using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;

namespace NHSD.GPIT.BuyingCatalogue.Services.Users
{
    public sealed class UsersService : IUsersService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public UsersService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<AspNetUser> GetUser(int userId)
        {
            return dbContext.AspNetUsers.SingleAsync(u => u.Id == userId);
        }

        public async Task<List<AspNetUser>> GetAllUsersForOrganisation(int organisationId)
        {
            return await dbContext.AspNetUsers.Where(u => u.PrimaryOrganisationId == organisationId).ToListAsync();
        }

        public async Task EnableOrDisableUser(int userId, bool disabled)
        {
            var user = await dbContext.AspNetUsers.SingleAsync(u => u.Id == userId);
            user.Disabled = disabled;
            await dbContext.SaveChangesAsync();
        }
    }
}
