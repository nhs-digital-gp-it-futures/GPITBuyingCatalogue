using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Configuration
{
    internal sealed class FilterHostingTypeEntityTypeConfiguration : IEntityTypeConfiguration<FilterHostingType>
    {
        public void Configure(EntityTypeBuilder<FilterHostingType> builder)
        {
            builder.ToTable("FilterHostingTypes", Schemas.Filtering);

            builder.HasKey(fht => fht.FilterHostingTypeId);

            builder.Property(e => e.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(fht => fht.Filter)
                .WithMany(ht => ht.FilterHostingTypes)
                .HasForeignKey(e => e.FilterId)
                .HasConstraintName("FK_FilterHostingTypes_Filter");

            builder.Property(fht => fht.HostingType)
                .HasConversion<int>()
                .HasColumnName("HostingTypeId");

            builder.HasOne(e => e.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.LastUpdatedBy)
                .HasConstraintName("FK_FilterHostingTypes_LastUpdatedBy");
        }
    }
}
