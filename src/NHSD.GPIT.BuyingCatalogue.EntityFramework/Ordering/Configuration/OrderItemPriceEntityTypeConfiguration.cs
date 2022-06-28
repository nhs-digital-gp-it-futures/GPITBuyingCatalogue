using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Configuration
{
    internal sealed class OrderItemPriceEntityTypeConfiguration : IEntityTypeConfiguration<OrderItemPrice>
    {
        public void Configure(EntityTypeBuilder<OrderItemPrice> builder)
        {
            builder.ToTable("OrderItemPrices", Schemas.Ordering);

            builder.HasKey(oipp => new { oipp.OrderId, oipp.CatalogueItemId });

            builder.Property(x => x.CataloguePriceId)
                .IsRequired();

            builder.Property(oip => oip.ProvisioningType)
                .HasConversion<int>()
                .HasColumnName("ProvisioningTypeId");

            builder.Property(oip => oip.CataloguePriceType)
                .HasConversion<int>()
                .HasColumnName("CataloguePriceTypeId");

            builder.Property(oip => oip.CataloguePriceCalculationType)
                .HasConversion<int>()
                .HasColumnName("CataloguePriceCalculationTypeId");

            builder.Property(p => p.CataloguePriceQuantityCalculationType)
                .HasConversion<int>()
                .HasColumnName("CataloguePriceQuantityCalculationTypeId");

            builder.Property(oip => oip.CurrencyCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(oip => oip.Description)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(oip => oip.RangeDescription)
                .HasMaxLength(100);

            builder.Property(oip => oip.EstimationPeriod)
                .HasConversion<int>()
                .HasColumnName("EstimationPeriodId");

            builder.Property(oipt => oipt.LastUpdated)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(oipp => oipp.OrderItem)
                .WithOne(oi => oi.OrderItemPrice)
                .HasForeignKey<OrderItemPrice>(oip => new { oip.OrderId, oip.CatalogueItemId })
                .HasConstraintName("FK_OrderItemPrices_OrderItem")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(oip => oip.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(oip => oip.LastUpdatedBy)
                .HasConstraintName("FK_OrderItemPrices_LastUpdatedBy");
        }
    }
}
