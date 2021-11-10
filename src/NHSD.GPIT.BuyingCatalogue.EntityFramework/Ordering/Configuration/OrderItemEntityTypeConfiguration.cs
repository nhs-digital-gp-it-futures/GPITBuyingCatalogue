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
            builder.ToTable("OrderItems", Schemas.Ordering);

            builder.HasKey(oi => new { oi.OrderId, oi.CatalogueItemId })
                .HasName("PK_OrderItem");

            builder.Property(e => e.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(oi => oi.Created).HasDefaultValue(DateTime.UtcNow);
            builder.Property(oi => oi.DefaultDeliveryDate).HasColumnType("date");
            builder.Property(oi => oi.EstimationPeriod)
                .HasConversion<int>()
                .HasColumnName("EstimationPeriodId");

            builder.Property(oi => oi.LastUpdated).HasDefaultValue(DateTime.UtcNow);
            builder.Property(oi => oi.LastUpdatedBy);
            builder.Property(oi => oi.Price).HasColumnType("decimal(18, 4)");

            builder.HasOne(oi => oi.CatalogueItem)
                .WithMany()
                .HasForeignKey(oi => oi.CatalogueItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderItems_CatalogueItem");

            builder.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .HasConstraintName("FK_OrderItems_Order");

            builder.HasOne(oi => oi.CataloguePrice)
                .WithMany()
                .HasForeignKey(oi => oi.PriceId)
                .HasConstraintName("FK_OrderItems_PriceId");

            builder.HasOne(oi => oi.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(oi => oi.LastUpdatedBy)
                .HasConstraintName("FK_OrderItems_LastUpdatedBy");
        }
    }
}
