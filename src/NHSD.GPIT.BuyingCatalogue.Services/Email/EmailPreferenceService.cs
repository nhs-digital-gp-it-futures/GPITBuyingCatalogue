using System;
using System.Collections.Generic;
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
            var user = await dbContext.AspNetUsers.Include(x => x.AspNetUserRoles)
                .ThenInclude(x => x.Role)
                .FirstOrDefaultAsync(x => x.Id == userId);

            var userRoles = user.AspNetUserRoles.Select(x => x.Role).ToList();

            var emailPreferences = await dbContext
                .EmailPreferenceTypes
                .Select(e => new UserEmailPreferenceModel(
                    e.EmailPreferenceTypeAsEnum,
                    e.DefaultEnabled,
                    e.UserPreferences.FirstOrDefault(u => u.UserId == user.Id) != null ? e.UserPreferences.FirstOrDefault(u => u.UserId == user.Id).Enabled : null,
                    e.RoleType))
                .ToListAsync();

            return emailPreferences
                .Where(x => x.RoleType.IsRoleMatch(userRoles))
                .ToList();
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
