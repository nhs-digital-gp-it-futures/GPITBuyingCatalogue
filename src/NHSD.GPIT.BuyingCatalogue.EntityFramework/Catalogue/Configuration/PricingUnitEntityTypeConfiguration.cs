using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class PricingUnitEntityTypeConfiguration : IEntityTypeConfiguration<PricingUnit>
    {
        public void Configure(EntityTypeBuilder<PricingUnit> builder)
        {
            builder.ToTable("PricingUnits", Schemas.Catalogue);

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Description)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Definition)
                .HasMaxLength(1000);

            builder.Property(u => u.RangeDescription)
                .HasMaxLength(100);

            builder.Property(u => u.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(u => u.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(u => u.LastUpdatedBy)
                .HasConstraintName("FK_PricingUnits_LastUpdatedBy");
        }
    }
}
