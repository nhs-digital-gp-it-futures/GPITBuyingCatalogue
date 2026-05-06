using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    public class
        OrderItemSublocationRecipientEntityTypeConfiguration : IEntityTypeConfiguration<OrderItemSublocationRecipient>
    {
        public void Configure(EntityTypeBuilder<OrderItemSublocationRecipient> builder)
        {
            builder.ToTable("OrderItemSublocationRecipientsV2", Schemas.Ordering);

            builder.HasKey(x => x.Id).HasName("PK_OrderItemSublocationRecipientsV2");

            builder.Property(x => x.OrderId).IsRequired();

            builder.Property(x => x.ParentSublocationOdsCode).HasMaxLength(10);

            builder.Property(x => x.RecipientOdsCode).HasMaxLength(10);

            builder.Property(x => x.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(x => x.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(x => x.LastUpdatedBy)
                .HasConstraintName("FK_OrderItemSublocationRecipientsV2_LastUpdatedBy");

            builder.HasOne(x => x.Recipient)
                .WithMany(y => y.OrderItemSublocationRecipients)
                .HasForeignKey(x => new { x.OrderId, x.ParentSublocationOdsCode, x.RecipientOdsCode })
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrderItemSublocationRecipientsV2_SublocationRecipient");

            builder.HasOne(x => x.OrderItem)
                .WithMany()
                .HasForeignKey(x => x.OrderItemId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_OrderItemSublocationRecipientsV2_OrderItem");
        }
    }
}
