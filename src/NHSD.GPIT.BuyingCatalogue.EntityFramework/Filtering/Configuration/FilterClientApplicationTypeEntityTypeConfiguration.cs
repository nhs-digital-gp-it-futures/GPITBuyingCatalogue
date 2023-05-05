using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Configuration
{
    internal sealed class FilterClientApplicationTypeEntityTypeConfiguration : IEntityTypeConfiguration<FilterClientApplicationType>
    {
        public void Configure(EntityTypeBuilder<FilterClientApplicationType> builder)
        {
            builder.ToTable("FilterClientApplicationTypes", Schemas.Filtering);

            builder.HasKey(fcat => fcat.FilterClientApplicationTypeId);

            builder.Property(e => e.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(fcat => fcat.Filter)
                .WithMany(cat => cat.FilterClientApplicationTypes)
                .HasForeignKey(e => e.FilterId)
                .HasConstraintName("FK_FilterClientApplicationTypes_Filter");

            builder.Property(p => p.ClientApplicationType)
                .HasConversion<int>()
                .HasColumnName("ClientApplicationTypeId");

            builder.HasOne(e => e.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.LastUpdatedBy)
                .HasConstraintName("FK_FilterClientApplicationTypes_LastUpdatedBy");
        }
    }
}
