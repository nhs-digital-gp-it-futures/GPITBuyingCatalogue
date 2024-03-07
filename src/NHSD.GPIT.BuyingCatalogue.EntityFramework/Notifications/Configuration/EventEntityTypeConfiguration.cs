using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Configuration
{
    internal sealed class EventEntityTypeConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Events", Schemas.Notifications);

            builder.HasKey(c => c.Id);

            builder.HasOne(o => o.ManagedEmailPreference)
                .WithMany()
                .HasForeignKey(o => o.ManagedEmailPreferenceId)
                .HasConstraintName("FK_Events_ManagedEmailPreference");
        }
    }
}
