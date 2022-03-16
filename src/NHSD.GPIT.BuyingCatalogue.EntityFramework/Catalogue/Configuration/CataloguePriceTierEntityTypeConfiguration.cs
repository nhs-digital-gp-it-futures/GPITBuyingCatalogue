using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class CataloguePriceTierEntityTypeConfiguration : IEntityTypeConfiguration<CataloguePriceTier>
    {
        public void Configure(EntityTypeBuilder<CataloguePriceTier> builder)
        {
            builder.ToTable("CataloguePriceTiers", Schemas.Catalogue);

            builder.HasKey(pt => pt.Id);

            builder.Property(pt => pt.Id).UseIdentityColumn();

            builder.Property(pt => pt.CataloguePriceId)
                .IsRequired();

            builder.Property(pt => pt.LowerRange)
                .IsRequired();

            builder.Property(pt => pt.Price)
                .IsRequired()
                .HasPrecision(18, 4);

            builder.Property(pt => pt.LastUpdated)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(pt => pt.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(pt => pt.LastUpdatedBy)
                .HasConstraintName("FK_CataloguePriceTiers_LastUpdatedBy");

            builder.HasOne(pt => pt.CataloguePrice)
                .WithMany(p => p.CataloguePriceTiers)
                .HasForeignKey(pt => pt.CataloguePriceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
