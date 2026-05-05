using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal sealed class OrderItemEntityTypeConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItemsV2", Schemas.Ordering);

            builder.HasKey(oi => oi.Id).HasName("PK_OrderItemsV2");

            builder.Property(oi => oi.EstimationPeriod)
                .HasConversion<int>()
                .HasColumnName("EstimationPeriodId");

            builder.Property(e => e.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(oi => oi.Created).HasDefaultValue(DateTime.UtcNow);

            builder.Property(oi => oi.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(oi => oi.CatalogueItem)
                .WithMany()
                .HasForeignKey(oi => oi.CatalogueItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItemsV2_CatalogueItem");

            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .HasConstraintName("FK_OrderItemsV2_Order");

            builder.HasOne(oi => oi.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(oi => oi.LastUpdatedBy)
                .HasConstraintName("FK_OrderItemsV2_LastUpdatedBy");

            builder.HasOne(oi => oi.Parent)
                .WithMany(oi => oi.Services)
                .HasForeignKey(oi => oi.ParentId)
                .HasConstraintName("FK_OrderItemsV2_Parent")
                .IsRequired(false)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
