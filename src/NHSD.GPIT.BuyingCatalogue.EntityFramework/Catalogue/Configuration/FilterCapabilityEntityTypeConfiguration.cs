using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class FilterCapabilityEntityTypeConfiguration : IEntityTypeConfiguration<FilterCapability>
    {
        public void Configure(EntityTypeBuilder<FilterCapability> builder)
        {
            builder.ToTable("FilterCapabilities", Schemas.Catalogue);

            builder.HasKey(fe => new { fe.FilterId, fe.CapabilityId });

            builder.Property(e => e.FilterId).HasMaxLength(10);
            builder.Property(e => e.CapabilityId).HasMaxLength(10);
            builder.Property(e => e.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne<Filter>()
                .WithMany()
                .HasForeignKey(e => e.FilterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FilterCapabilities_Filter");

            builder.HasOne(e => e.Capability)
                .WithMany()
                .HasForeignKey(e => e.CapabilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FilterCapabilities_Capability");

            builder.HasOne(e => e.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.LastUpdatedBy)
                .HasConstraintName("FK_FilterCapabilities_LastUpdatedBy");
        }
    }
}
