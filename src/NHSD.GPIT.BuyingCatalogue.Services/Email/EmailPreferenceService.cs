using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NHSD.GPIT.BuyingCatalogue.EntityFramework;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;
using NHSD.GPIT.BuyingCatalogue.ServiceContracts.Email;

namespace NHSD.GPIT.BuyingCatalogue.Services.Email
{
    public class EmailPreferenceService : IEmailPreferenceService
    {
        private readonly BuyingCatalogueDbContext dbContext;

        public EmailPreferenceService(BuyingCatalogueDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<List<UserEmailPreferenceModel>> Get(int userId)
        {
            return await dbContext
                .EmailPreferenceTypes
                .Select(e => new UserEmailPreferenceModel(
                    e.EmailPreferenceTypeAsEnum,
                    e.DefaultEnabled,
                    e.UserPreferences.FirstOrDefault(u => u.UserId == userId) != null ? e.UserPreferences.FirstOrDefault(u => u.UserId == userId).Enabled : null))
                .ToListAsync();
        }

        public async Task Save(int userId, ICollection<UserEmailPreferenceModel> preferences)
        {
            ArgumentNullException.ThrowIfNull(preferences);

            foreach (var userPreference in preferences)
            {
                var userEmailPreference = await dbContext
                    .UserEmailPreferences
                    .FirstOrDefaultAsync(e => e.UserId == userId && e.EmailPreferenceTypeId == (int)userPreference.EmailPreferenceType);
                if (userEmailPreference != null)
                {
                    userEmailPreference.Enabled = userPreference.Enabled;
                }
                else
                {
                    dbContext.UserEmailPreferences.Add(new UserEmailPreference()
                    {
                        UserId = userId,
                        EmailPreferenceTypeId = (int)userPreference.EmailPreferenceType,
                        Enabled = userPreference.Enabled,
                    });
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
