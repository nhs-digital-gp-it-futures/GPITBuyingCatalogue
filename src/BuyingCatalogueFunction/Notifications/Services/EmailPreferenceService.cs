using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;
using IEmailPreferenceService = BuyingCatalogueFunction.Notifications.Interfaces.IEmailPreferenceService;

namespace BuyingCatalogueFunction.Notifications.Services;

public class EmailPreferenceService : IEmailPreferenceService
{
    private readonly BuyingCatalogueDbContext dbContext;

    public EmailPreferenceService(
        BuyingCatalogueDbContext dbContext)
    {
        this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task<bool> ShouldTriggerForUser(EmailPreferenceType emailPreferenceType, int userId)
    {
        var userPreference = await dbContext
            .UserEmailPreferences
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.EmailPreferenceTypeId == emailPreferenceType.Id
                                      && u.UserId == userId);

        var userRoles = await dbContext.UserRoles
            .Where(x => x.UserId == userId)
            .Select(x => x.Role)
            .AsNoTracking()
            .ToListAsync();

        if (!emailPreferenceType.RoleType.IsRoleMatch(userRoles))
            return false;

        return userPreference?.Enabled ?? emailPreferenceType.DefaultEnabled;
    }

    public async Task<EmailPreferenceType> GetDefaultEmailPreference(EmailPreferenceTypeEnum eventType)
    {
        return await dbContext
            .EmailPreferenceTypes
            .Include(x => x.SupportedEventTypes)
            .FirstOrDefaultAsync(u => u.Id == (int)eventType);
    }
}
