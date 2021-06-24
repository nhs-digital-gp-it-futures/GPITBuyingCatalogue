using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal sealed class OrderItemRecipientEntityTypeConfiguration : IEntityTypeConfiguration<OrderItemRecipient>
    {
        public void Configure(EntityTypeBuilder<OrderItemRecipient> builder)
        {
            builder.ToTable("OrderItemRecipients", "ordering");

            builder.HasKey(r => new { r.OrderId, r.CatalogueItemId, r.OdsCode });

            builder.Property(r => r.CatalogueItemId)
                .HasMaxLength(14);

            // TODO: reapply after converting CatalogueItemId to correct type
            // .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));
            builder.Property(r => r.OdsCode).HasMaxLength(8);
            builder.Property(r => r.DeliveryDate).HasColumnType("date");

            builder.HasOne(r => r.Recipient)
                .WithMany(r => r.OrderItemRecipients)
                .HasForeignKey(r => r.OdsCode)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItemRecipients_OdsCode");

            builder.HasOne(r => r.OrderItem)
                .WithMany(oi => oi.OrderItemRecipients)
                .HasForeignKey(r => new { r.OrderId, r.CatalogueItemId })
                .HasConstraintName("FK_OrderItemRecipients_OrderItem");
        }
    }
}
