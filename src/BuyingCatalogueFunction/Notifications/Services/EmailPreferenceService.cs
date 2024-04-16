using System;
using System.Threading.Tasks;
using BuyingCatalogueFunction.Notifications.Interfaces;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

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
            .FirstOrDefaultAsync(u => u.EmailPreferenceTypeId == emailPreferenceType.Id
                                      && u.UserId == userId);

        return userPreference?.Enabled ?? emailPreferenceType.DefaultEnabled;
    }

    public async Task<EmailPreferenceType> GetDefaultEmailPreference(EmailPreferenceTypeEnum eventType)
    {
        return await dbContext
            .EmailPreferenceTypes
            .FirstOrDefaultAsync(u => u.Id == (int)eventType);
    }
}
