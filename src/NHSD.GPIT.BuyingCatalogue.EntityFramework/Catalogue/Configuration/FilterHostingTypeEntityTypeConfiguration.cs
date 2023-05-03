using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class FilterHostingTypeEntityTypeConfiguration : IEntityTypeConfiguration<FilterHostingType>
    {
        public void Configure(EntityTypeBuilder<FilterHostingType> builder)
        {
            builder.ToTable("FilterHostingTypes", Schemas.Catalogue);

            builder.HasKey(fe => new { fe.FilterId, fe.HostingTypeId });

            builder.Property(e => e.FilterId).IsRequired().HasMaxLength(10);
            builder.Property(e => e.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne<Filter>()
                .WithMany()
                .HasForeignKey(e => e.FilterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FilterHostingTypes_Filter");

            builder.Property(p => p.HostingType)
                .HasConversion<int>()
                .HasColumnName("HostingTypeId");

            builder.HasOne(e => e.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.LastUpdatedBy)
                .HasConstraintName("FK_FilterHostingTypes_LastUpdatedBy");
        }
    }
}
