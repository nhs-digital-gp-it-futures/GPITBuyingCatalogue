using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;

namespace NHSD.GPIT.BuyingCatalogue.Services.Users
{
    public sealed class UsersService(
        BuyingCatalogueDbContext dbContext,
        AccountManagementSettings accountManagementSettings,
        TermsOfUseSettings settings)
        : IUsersService
    {
        private readonly BuyingCatalogueDbContext dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        private readonly AccountManagementSettings accountManagementSettings = accountManagementSettings ?? throw new ArgumentNullException(nameof(accountManagementSettings));
        private readonly TermsOfUseSettings settings = settings ?? throw new ArgumentNullException(nameof(settings));

        public Task<AspNetUser> GetUser(int userId)
        {
            return dbContext.AspNetUsers
                .Include(x => x.PrimaryOrganisation)
                .Include(x => x.AspNetUserRoles)
                .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<List<AspNetUser>> GetAllUsers()
        {
            return await dbContext.AspNetUsers
                .Include(x => x.PrimaryOrganisation)
                .Include(x => x.AspNetUserRoles)
                .ThenInclude(x => x.Role)
                .OrderBy(x => x.Disabled)
                .ThenBy(x => x.LastName)
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
                .Include(x => x.AspNetUserRoles)
                .ThenInclude(x => x.Role)
                .OrderBy(x => x.Disabled)
                .ThenBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();
        }

        public async Task UpdateUser(int userId, string firstName, string lastName, string email, bool disabled, string organisationFunction, int organisationId)
        {
            var user = await GetUser(userId);

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            user.UserName = email;
            user.Disabled = disabled;
            user.PrimaryOrganisationId = organisationId;

            var role = await dbContext.Roles.FirstOrDefaultAsync(x => x.Name == organisationFunction);

            user.AspNetUserRoles = new List<AspNetUserRole> { new() { RoleId = role.Id }, };

            await dbContext.SaveChangesAsync();
        }

        public async Task SetTermsOfUse(int userId, bool hasOptedInUserResearch)
        {
            var user = await dbContext.AspNetUsers.FirstOrDefaultAsync(x => x.Id == userId);
            if (user is null) return;

            if (!user.HasAcceptedLatestTermsOfUse(settings.RevisionDate))
            {
                user.AcceptedTermsOfUseDate = DateTime.UtcNow;
            }

            user.HasOptedInUserResearch = hasOptedInUserResearch;

            await dbContext.SaveChangesAsync();
        }

        public async Task<bool> EmailAddressExists(string emailAddress, int userId = 0)
        {
            var testAddress = (emailAddress ?? string.Empty).ToUpperInvariant();
            var user = await dbContext.AspNetUsers
                .FirstOrDefaultAsync(x => x.Id != userId && x.NormalizedEmail == testAddress);

            return user != null;
        }

        public async Task<bool> IsAccountManagerLimit(int organisationId, int userId = 0)
        {
            var users = await dbContext.AspNetUsers
                .Include(x => x.AspNetUserRoles)
                .ThenInclude(x => x.Role)
                .AsAsyncEnumerable()
                .CountAsync(
                    u => u.Id != userId
                        && u.PrimaryOrganisationId == organisationId
                        && u.GetRoleName() == OrganisationFunction.AccountManager.Name
                        && !u.Disabled);

            return users >= accountManagementSettings.MaximumNumberOfAccountManagers;
        }
    }
}
