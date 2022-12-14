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

            builder.Property(oipt => oipt.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

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
                .HasForeignKey(oipt => new { oipt.OrderId, oipt.CatalogueItemId })
                .HasConstraintName("FK_OrderItemPriceTiers_OrderItemPrices")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
