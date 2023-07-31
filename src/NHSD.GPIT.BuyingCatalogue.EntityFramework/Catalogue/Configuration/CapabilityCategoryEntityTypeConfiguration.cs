using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class CapabilityCategoryEntityTypeConfiguration : IEntityTypeConfiguration<CapabilityCategory>
    {
        public void Configure(EntityTypeBuilder<CapabilityCategory> builder)
        {
            builder.ToTable("CapabilityCategories", Schemas.Catalogue);

            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).ValueGeneratedOnAdd();

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Description)
                .HasMaxLength(200);

            builder.Property(c => c.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasIndex(c => c.Name, "AK_CapabilityCategories_Name")
                .IsUnique();

            builder.HasOne(c => c.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(c => c.LastUpdatedBy)
                .HasConstraintName("FK_CapabilityCategories_LastUpdatedBy");
        }
    }
}
