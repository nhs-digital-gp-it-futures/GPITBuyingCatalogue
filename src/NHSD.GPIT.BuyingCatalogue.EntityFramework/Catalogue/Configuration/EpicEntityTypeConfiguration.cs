using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class EpicEntityTypeConfiguration : IEntityTypeConfiguration<Epic>
    {
        public void Configure(EntityTypeBuilder<Epic> builder)
        {
            builder.ToTable("Epics", Schemas.Catalogue);

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).HasMaxLength(10);
            builder.Property(e => e.CompliancyLevel)
                .HasConversion<int>()
                .HasColumnName("CompliancyLevelId");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(e => e.IsActive).IsRequired();
            builder.Property(e => e.SupplierDefined).IsRequired().HasDefaultValue(false);
            builder.Property(e => e.LastUpdated).HasDefaultValue(DateTime.UtcNow);
            builder.Property(e => e.LastUpdatedBy);

            builder.HasOne<Capability>()
                .WithMany(c => c.Epics)
                .HasForeignKey(e => e.CapabilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Epics_Capability");

            builder.HasOne(e => e.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.LastUpdatedBy)
                .HasConstraintName("FK_Epics_LastUpdatedBy");
        }
    }
}
