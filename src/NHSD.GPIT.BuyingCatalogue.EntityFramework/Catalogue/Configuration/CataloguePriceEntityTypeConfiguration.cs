using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class CataloguePriceEntityTypeConfiguration : IEntityTypeConfiguration<CataloguePrice>
    {
        public void Configure(EntityTypeBuilder<CataloguePrice> builder)
        {
            builder.ToTable("CataloguePrices", Schemas.Catalogue);

            builder.Property(p => p.CatalogueItemId)
                .IsRequired()
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(p => p.CataloguePriceType)
                .HasConversion<int>()
                .HasColumnName("CataloguePriceTypeId");

            builder.Property(p => p.CurrencyCode)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(p => p.Price).HasColumnType("decimal(18, 4)");
            builder.Property(p => p.ProvisioningType)
                .HasConversion<int>()
                .HasColumnName("ProvisioningTypeId");

            builder.Property(p => p.TimeUnit)
                .HasConversion<int>()
                .HasColumnName("TimeUnitId");

            builder.Property(p => p.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(p => p.CatalogueItem)
                .WithMany(i => i.CataloguePrices)
                .HasForeignKey(p => p.CatalogueItemId);

            builder.HasOne(p => p.PricingUnit)
                .WithMany()
                .HasForeignKey(p => p.PricingUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(p => p.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(p => p.LastUpdatedBy)
                .HasConstraintName("FK_CataloguePrices_LastUpdatedBy");
        }
    }
}
