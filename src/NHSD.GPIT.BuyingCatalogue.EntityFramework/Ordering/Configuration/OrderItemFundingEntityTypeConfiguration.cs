using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal sealed class OrderItemFundingEntityTypeConfiguration : IEntityTypeConfiguration<OrderItemFunding>
    {
        public void Configure(EntityTypeBuilder<OrderItemFunding> builder)
        {
            builder.ToTable("OrderItemFunding", Schemas.Ordering);

            builder.HasKey(oif => new { oif.OrderId, oif.CatalogueItemId });

            builder.Property(oif => oif.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(oif => oif.TotalPrice)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(oif => oif.CentralAllocation)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(oif => oif.LocalAllocation)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(oif => oif.LastUpdated)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

            builder.Property(oif => oif.LastUpdatedBy)
                .IsRequired();

            builder.HasOne(oif => oif.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(oif => oif.LastUpdatedBy)
                .HasConstraintName("FK_OrderItemFunding_LastUpdatedBy");

            builder.HasOne(oif => oif.OrderItem)
                .WithOne(oi => oi.OrderItemFunding)
                .HasForeignKey<OrderItemFunding>(oif => new { oif.OrderId, oif.CatalogueItemId })
                .HasConstraintName("FK_OrderItemFunding_OrderItems")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
