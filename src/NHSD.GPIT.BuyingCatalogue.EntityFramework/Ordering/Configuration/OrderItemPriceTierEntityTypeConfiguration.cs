using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal class OrderItemPriceTierEntityTypeConfiguration : IEntityTypeConfiguration<OrderItemPriceTier>
    {
        public void Configure(EntityTypeBuilder<OrderItemPriceTier> builder)
        {
            builder.ToTable("OrderItemPriceTiers", Schemas.Ordering);

            builder.HasKey(oipt => oipt.Id);

            builder.Property(oipt => oipt.Price)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(oipt => oipt.ListPrice)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(oipt => oipt.LastUpdated)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(oipt => oipt.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(oipt => oipt.LastUpdatedBy);

            builder.HasOne(oipt => oipt.OrderItemPrice)
                .WithMany(oip => oip.OrderItemPriceTiers)
                .HasForeignKey(oipt => oipt.OrderItemPriceId)
                .HasConstraintName("FK_OrderItemPriceTiers_OrderItemPricesV2")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
