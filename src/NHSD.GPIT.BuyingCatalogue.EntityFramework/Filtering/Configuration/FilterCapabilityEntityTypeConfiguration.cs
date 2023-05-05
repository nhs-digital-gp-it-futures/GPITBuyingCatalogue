using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Configuration
{
    internal sealed class FilterCapabilityEntityTypeConfiguration : IEntityTypeConfiguration<FilterCapability>
    {
        public void Configure(EntityTypeBuilder<FilterCapability> builder)
        {
            builder.ToTable("FilterCapabilities", Schemas.Filtering);

            builder.HasKey(fe => new { fe.FilterId, fe.CapabilityId }).HasName("PK_FilterCapabilities");

            builder.Property(e => e.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(fc => fc.Filter)
                .WithMany(f => f.FilterCapabilities)
                .HasForeignKey(fc => fc.FilterId)
                .HasConstraintName("FK_FilterCapabilities_Filter");

            builder.HasOne(fc => fc.Capability)
                .WithMany()
                .HasForeignKey(fc => fc.CapabilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FilterCapabilities_Capability");

            builder.HasOne(e => e.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.LastUpdatedBy)
                .HasConstraintName("FK_FilterCapabilities_LastUpdatedBy");
        }
    }
}
