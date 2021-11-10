using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class CatalogueItemCapabilityStatusEntityTypeConfiguration : IEntityTypeConfiguration<CatalogueItemCapabilityStatus>
    {
        public void Configure(EntityTypeBuilder<CatalogueItemCapabilityStatus> builder)
        {
            builder.ToTable("CatalogueItemCapabilityStatus", Schemas.Catalogue);

            builder.Property(s => s.Id).ValueGeneratedNever();
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(16);

            builder.Property(s => s.LastUpdated).HasDefaultValue(DateTime.UtcNow);
            builder.Property(s => s.LastUpdatedBy);

            builder.HasIndex(s => s.Name, "AK_CatalogueItemCapabilityStatus_Name")
                .IsUnique();

            builder.HasOne(s => s.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(s => s.LastUpdatedBy)
                .HasConstraintName("FK_Capabilities_LastUpdatedBy");
        }
    }
}
