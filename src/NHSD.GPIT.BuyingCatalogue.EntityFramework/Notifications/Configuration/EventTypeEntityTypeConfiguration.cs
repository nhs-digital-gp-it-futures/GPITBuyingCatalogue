using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Configuration
{
    internal sealed class EventTypeEntityTypeConfiguration : IEntityTypeConfiguration<EventType>
    {
        public void Configure(EntityTypeBuilder<EventType> builder)
        {
            builder.ToTable("EventTypes", Schemas.Notifications);

            builder.HasKey(c => c.Id);

            builder.HasOne(o => o.EmailPreferenceType)
                .WithMany()
                .HasForeignKey(o => o.EmailPreferenceTypeId)
                .HasConstraintName("FK_EventTypes_EmailPreferenceType");
        }
    }
}
