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
            return dbContext.AspNetUsers
                .Include(x => x.PrimaryOrganisation)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<List<AspNetUser>> GetAllUsers()
        {
            return await dbContext.AspNetUsers
                .Include(x => x.PrimaryOrganisation)
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();
        }

        public async Task<List<AspNetUser>> GetAllUsersBySearchTerm(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentNullException(nameof(searchTerm));

            var users = await GetAllUsers();

            return users
                .Where(x => x.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                    || x.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public async Task<List<AspNetUser>> GetAllUsersForOrganisation(int organisationId)
        {
            return await dbContext.AspNetUsers
                .Where(u => u.PrimaryOrganisationId == organisationId)
                .ToListAsync();
        }

        public async Task EnableOrDisableUser(int userId, bool disabled)
        {
            var user = await dbContext.AspNetUsers.SingleAsync(u => u.Id == userId);

            user.Disabled = disabled;

            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateUserAccountType(int userId, string organisationFunction)
        {
            var user = await dbContext.AspNetUsers.SingleAsync(u => u.Id == userId);

            user.OrganisationFunction = organisationFunction;

            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateUserDetails(int userId, string firstName, string lastName, string email)
        {
            var user = await dbContext.AspNetUsers.SingleAsync(u => u.Id == userId);

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            user.NormalizedEmail = email?.ToUpperInvariant();
            user.NormalizedUserName = email?.ToUpperInvariant();
            user.UserName = email;

            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateUserOrganisation(int userId, int organisationId)
        {
            var user = await dbContext.AspNetUsers.SingleAsync(u => u.Id == userId);

            user.PrimaryOrganisationId = organisationId;

            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> EmailAddressExists(string emailAddress, int userId = 0)
        {
            var testAddress = (emailAddress ?? string.Empty).ToUpperInvariant();
            var user = await dbContext.AspNetUsers
                .FirstOrDefaultAsync(x => x.Id != userId && x.NormalizedEmail == testAddress);

            return user != null;
        }
    }
}
