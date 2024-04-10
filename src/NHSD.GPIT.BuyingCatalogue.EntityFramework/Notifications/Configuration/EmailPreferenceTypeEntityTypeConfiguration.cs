using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Configuration
{
    internal sealed class EmailPreferenceTypeEntityTypeConfiguration : IEntityTypeConfiguration<EmailPreferenceType>
    {
        public void Configure(EntityTypeBuilder<EmailPreferenceType> builder)
        {
            builder.ToTable("EmailPreferenceTypes", Schemas.Notifications);

            builder.HasKey(c => c.Id);

            builder.HasMany(x => x.UserPreferences)
                .WithOne()
                .HasForeignKey(x => x.EmailPreferenceTypeId);
        }
    }
}
