using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class FilterClientApplicationTypeEntityTypeConfiguration : IEntityTypeConfiguration<FilterClientApplicationType>
    {
        public void Configure(EntityTypeBuilder<FilterClientApplicationType> builder)
        {
            builder.ToTable("FilterClientApplicationTypes", Schemas.Catalogue);

            builder.HasKey(fe => new { fe.FilterId, fe.ClientApplicationTypeId });

            builder.Property(e => e.FilterId).IsRequired().HasMaxLength(10);
            builder.Property(e => e.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne<Filter>()
                .WithMany()
                .HasForeignKey(e => e.FilterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
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
