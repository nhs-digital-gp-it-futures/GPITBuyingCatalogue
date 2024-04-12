using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Configuration
{
    internal sealed class UserEmailPreferenceEntityTypeConfiguration : IEntityTypeConfiguration<UserEmailPreference>
    {
        public void Configure(EntityTypeBuilder<UserEmailPreference> builder)
        {
            builder.ToTable("UserEmailPreferences", Schemas.Notifications);

            builder.HasKey(c => new { c.UserId, c.EmailPreferenceTypeId });
        }
    }
}
