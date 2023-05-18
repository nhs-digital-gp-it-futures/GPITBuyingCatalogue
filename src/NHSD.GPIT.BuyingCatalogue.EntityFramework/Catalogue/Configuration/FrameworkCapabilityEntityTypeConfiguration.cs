using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class FrameworkCapabilityEntityTypeConfiguration : IEntityTypeConfiguration<FrameworkCapability>
    {
        public void Configure(EntityTypeBuilder<FrameworkCapability> builder)
        {
            builder.ToTable("FrameworkCapabilities", Schemas.Catalogue);

            builder.HasKey(f => new { f.FrameworkId, f.CapabilityId });

            builder.Property(f => f.FrameworkId).HasMaxLength(36);
            builder.Property(f => f.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(f => f.Capability)
                .WithMany(c => c.FrameworkCapabilities)
                .HasForeignKey(d => d.CapabilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FrameworkCapabilities_Capability");

            builder.HasOne(f => f.Framework)
                .WithMany()
                .HasForeignKey(d => d.FrameworkId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FrameworkCapabilities_Framework");

            builder.HasOne(f => f.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(f => f.LastUpdatedBy)
                .HasConstraintName("FK_FrameworkCapabilities_LastUpdatedBy");
        }
    }
}
