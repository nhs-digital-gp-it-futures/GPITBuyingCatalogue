using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Configuration
{
    internal sealed class ManagedEmailPreferenceEntityTypeConfiguration : IEntityTypeConfiguration<ManagedEmailPreference>
    {
        public void Configure(EntityTypeBuilder<ManagedEmailPreference> builder)
        {
            builder.ToTable("ManagedEmailPreferences", Schemas.Notifications);

            builder.HasKey(c => c.Id);
        }
    }
}
