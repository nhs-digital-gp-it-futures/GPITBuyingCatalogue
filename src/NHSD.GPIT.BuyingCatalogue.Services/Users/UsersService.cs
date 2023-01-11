using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Flurl.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;

namespace NHSD.GPIT.BuyingCatalogue.Services.Users
{
    public sealed class UsersService : IUsersService
    {
        private readonly UserManager<AspNetUser> userManager;
        private readonly AccountManagementSettings accountManagementSettings;

        public UsersService(UserManager<AspNetUser> userManager, AccountManagementSettings accountManagementSettings)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            this.accountManagementSettings = accountManagementSettings ?? throw new ArgumentNullException(nameof(accountManagementSettings));
        }

        public Task<AspNetUser> GetUser(int userId)
        {
            return userManager.Users
                .Include(x => x.PrimaryOrganisation)
                .Include(x => x.AspNetUserRoles)
                .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<List<AspNetUser>> GetAllUsers()
        {
            return await userManager.Users
                .Include(x => x.PrimaryOrganisation)
                .Include(x => x.AspNetUserRoles)
                .ThenInclude(x => x.Role)
                .OrderBy(x => x.Disabled)
                .ThenBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();
        }

        public async Task<bool> HasRole(int userId, string role)
        {
            var user = await GetUser(userId);
            return await userManager.IsInRoleAsync(user, role);
        }

        public async Task<IList<string>> GetRoles(AspNetUser user)
        {
            return await userManager.GetRolesAsync(user);
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
            return await userManager.Users
                .Where(u => u.PrimaryOrganisationId == organisationId)
                .Include(x => x.AspNetUserRoles)
                .ThenInclude(x => x.Role)
                .OrderBy(x => x.Disabled)
                .ThenBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .ToListAsync();
        }

        public async Task EnableOrDisableUser(int userId, bool disabled)
        {
            var user = await userManager.Users.FirstAsync(u => u.Id == userId);

            user.Disabled = disabled;

            await userManager.UpdateAsync(user);
        }

        public async Task UpdateUserAccountType(int userId, string organisationFunction)
        {
            var user = await userManager.Users.FirstAsync(u => u.Id == userId);
            var userRoles = await userManager.GetRolesAsync(user);

            await userManager.RemoveFromRolesAsync(user, userRoles);
            await userManager.AddToRoleAsync(user, organisationFunction);

            await userManager.UpdateAsync(user);
        }

        public async Task UpdateUserDetails(int userId, string firstName, string lastName, string email)
        {
            var user = await userManager.Users.FirstAsync(u => u.Id == userId);

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            user.UserName = email;

            await userManager.UpdateAsync(user);
        }

        public async Task UpdateUserOrganisation(int userId, int organisationId)
        {
            var user = await userManager.Users.FirstAsync(u => u.Id == userId);

            user.PrimaryOrganisationId = organisationId;

            await userManager.UpdateAsync(user);
        }

        public async Task UpdateUser(int userId, string firstName, string lastName, string email, bool disabled, string organisationFunction, int organisationId)
        {
            var user = await userManager.Users.FirstAsync(u => u.Id == userId);

            user.FirstName = firstName;
            user.LastName = lastName;
            user.Email = email;
            user.UserName = email;
            user.Disabled = disabled;
            user.PrimaryOrganisationId = organisationId;

            var userRoles = await userManager.GetRolesAsync(user);

            await userManager.RemoveFromRolesAsync(user, userRoles);
            await userManager.AddToRoleAsync(user, organisationFunction);

            await userManager.UpdateAsync(user);
        }

        public async Task<bool> EmailAddressExists(string emailAddress, int userId = 0)
        {
            var testAddress = (emailAddress ?? string.Empty).ToUpperInvariant();
            var user = await userManager.Users
                .FirstOrDefaultAsync(x => x.Id != userId && x.NormalizedEmail == testAddress);

            return user != null;
        }

        public async Task<bool> IsAccountManagerLimit(int organisationId, int userId = 0)
        {
            var users = await userManager.Users
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
