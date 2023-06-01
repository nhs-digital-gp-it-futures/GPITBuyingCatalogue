using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.ValueGenerators;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class EpicEntityTypeConfiguration : IEntityTypeConfiguration<Epic>
    {
        public void Configure(EntityTypeBuilder<Epic> builder)
        {
            builder.ToTable("Epics", Schemas.Catalogue);

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .HasMaxLength(10)
                .HasValueGenerator<EpicIdValueGenerator>();

            builder.Property(e => e.Description)
                .HasMaxLength(1500);

            builder.Property(e => e.CompliancyLevel)
                .HasConversion<int>()
                .HasColumnName("CompliancyLevelId");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.IsActive).IsRequired();
            builder.Property(e => e.SupplierDefined).IsRequired().HasDefaultValue(false);
            builder.Property(e => e.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(e => e.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.LastUpdatedBy)
                .HasConstraintName("FK_Epics_LastUpdatedBy");
        }
    }
}
