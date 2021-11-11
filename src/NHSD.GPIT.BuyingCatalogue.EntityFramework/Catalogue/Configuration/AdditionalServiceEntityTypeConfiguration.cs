using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class AdditionalServiceEntityTypeConfiguration : IEntityTypeConfiguration<AdditionalService>
    {
        public void Configure(EntityTypeBuilder<AdditionalService> builder)
        {
            builder.ToTable("AdditionalServices", Schemas.Catalogue);

            builder.HasKey(a => a.CatalogueItemId);

            builder.Property(a => a.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(a => a.FullDescription).HasMaxLength(3000);
            builder.Property(a => a.SolutionId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(a => a.Summary).HasMaxLength(300);
            builder.Property(a => a.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(a => a.CatalogueItem)
                .WithOne(i => i.AdditionalService)
                .HasForeignKey<AdditionalService>(d => d.CatalogueItemId)
                .HasConstraintName("FK_AdditionalServices_CatalogueItem");

            builder.HasOne(a => a.Solution)
                .WithMany(s => s.AdditionalServices)
                .HasForeignKey(a => a.SolutionId)
                .HasConstraintName("FK_AdditionalServices_Solution");

            builder.HasOne(a => a.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(a => a.LastUpdatedBy)
                .HasConstraintName("FK_AdditionalServices_LastUpdatedBy");
        }
    }
}
