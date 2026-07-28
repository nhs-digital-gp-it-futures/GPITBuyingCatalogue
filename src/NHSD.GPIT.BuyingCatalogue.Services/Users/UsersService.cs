using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Users.Models;
using NHSD.GPIT.BuyingCatalogue.Framework.Extensions;
using NHSD.GPIT.BuyingCatalogue.Framework.Settings;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Identity;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Users;

namespace NHSD.GPIT.BuyingCatalogue.Services.Users;

public sealed class UsersService(
    UserManager<AspNetUser> userManager,
    AccountManagementSettings accountManagementSettings,
    AccountTemplateSettings accountTemplateSettings,
    IGovNotifyEmailService govNotifyEmailService) : IUsersService
{
    private readonly UserManager<AspNetUser> userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    private readonly AccountManagementSettings accountManagementSettings = accountManagementSettings ?? throw new ArgumentNullException(nameof(accountManagementSettings));
    private readonly AccountTemplateSettings accountTemplateSettings = accountTemplateSettings ?? throw new ArgumentNullException(nameof(accountTemplateSettings));
    private readonly IGovNotifyEmailService govNotifyEmailService = govNotifyEmailService ?? throw new ArgumentNullException(nameof(govNotifyEmailService));

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

    public async Task UpdateUser(UpdateUserRequest req)
    {
        ArgumentNullException.ThrowIfNull(req);

        var user = await userManager.Users.FirstAsync(u => u.Id == req.UserId);
        var isReactivation = user.Disabled && !req.Disabled;

        user.FirstName = req.FirstName;
        user.LastName = req.LastName;
        user.Email = req.Email;
        user.UserName = req.Email;
        user.Disabled = req.Disabled;
        user.PrimaryOrganisationId = req.OrganisationId;
        user.DeactivationReason = req.Disabled ? AccountDeactivationReason.Manual : null;
        user.ReactivationDate = req.ReactivationDate;

        if (isReactivation)
        {
            user.Events.Add(new AspNetUserEvent((int)EventTypeEnum.UserAccountReactivated));
            await govNotifyEmailService.SendEmailAsync(req.Email, accountTemplateSettings.AccountReactivationTemplateId, null);
        }

        var userRoles = await userManager.GetRolesAsync(user);

        await userManager.RemoveFromRolesAsync(user, userRoles);
        await userManager.AddToRoleAsync(user, req.OrganisationFunction);
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

    public async Task SendDeactivatedUserEmail(string email)
    {
        await govNotifyEmailService.SendEmailAsync(email, accountTemplateSettings.AccountDeactivationTemplateId, null);
    }
}
