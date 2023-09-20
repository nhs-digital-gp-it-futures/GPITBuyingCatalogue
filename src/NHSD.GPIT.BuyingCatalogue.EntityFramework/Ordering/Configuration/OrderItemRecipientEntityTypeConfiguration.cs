using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal sealed class OrderItemRecipientEntityTypeConfiguration : IEntityTypeConfiguration<OrderItemRecipient>
    {
        public void Configure(EntityTypeBuilder<OrderItemRecipient> builder)
        {
            builder.ToTable("OrderItemRecipients", Schemas.Ordering);

            builder.HasKey(r => new { r.OrderId, r.CatalogueItemId, r.OdsCode });

            builder.Property(r => r.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(r => r.OdsCode).HasMaxLength(8);
            builder.Property(r => r.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(r => r.Recipient)
                .WithMany(r => r.OrderItemRecipients)
                .HasForeignKey(r => new { r.OrderId, r.OdsCode })
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrderItemRecipients_OdsCode");

            builder.HasOne(r => r.OrderItem)
                .WithMany()
                .HasForeignKey(r => new { r.OrderId, r.CatalogueItemId })
                .HasConstraintName("FK_OrderItemRecipients_OrderItem");

            builder.HasOne(r => r.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(r => r.LastUpdatedBy)
                .HasConstraintName("FK_OrderItemRecipients_LastUpdatedBy");
        }
    }
}
