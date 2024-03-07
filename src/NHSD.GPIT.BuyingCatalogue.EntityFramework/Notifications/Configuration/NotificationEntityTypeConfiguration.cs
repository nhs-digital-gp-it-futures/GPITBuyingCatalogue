using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Notifications.Configuration
{
    internal sealed class NotificationEntityTypeConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications", Schemas.Notifications);

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            builder.Property(u => u.To).HasMaxLength(256).IsRequired();
            builder.Property(o => o.Created).HasDefaultValue(DateTime.UtcNow);

            builder.Property(i => i.NotificationType)
                .HasConversion<int>()
                .HasColumnName("NotificationTypeId");
        }
    }
}
