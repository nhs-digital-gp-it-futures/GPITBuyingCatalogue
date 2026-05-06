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
            builder.ToTable("OrderItemFundingV2", Schemas.Ordering);

            builder.HasKey(oif => oif.Id).HasName("PK_OrderItemFundingV2");

            builder.Property(oif => oif.OrderItemFundingType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(oif => oif.LastUpdated)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

            builder.Property(oif => oif.LastUpdatedBy)
                .IsRequired();

            builder.HasOne(oif => oif.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(oif => oif.LastUpdatedBy)
                .HasConstraintName("FK_OrderItemFunding_LastUpdatedByV2");

            builder.HasOne(oif => oif.OrderItem)
                .WithOne(oi => oi.OrderItemFunding)
                .HasForeignKey<OrderItemFunding>(oif => oif.OrderItemId)
                .HasConstraintName("FK_OrderItemFunding_OrderItemV2")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
