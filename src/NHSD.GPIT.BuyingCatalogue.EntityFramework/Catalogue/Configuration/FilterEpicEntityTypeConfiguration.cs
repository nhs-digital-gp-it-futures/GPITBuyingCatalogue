using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class FilterEpicEntityTypeConfiguration : IEntityTypeConfiguration<FilterEpic>
    {
        public void Configure(EntityTypeBuilder<FilterEpic> builder)
        {
            builder.ToTable("FilterEpics", Schemas.Catalogue);

            builder.HasKey(fe => new { fe.FilterId, fe.EpicId }).HasName("PK_FilterEpics");

            builder.Property(e => e.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(fe => fe.Filter)
                .WithMany(f => f.FilterEpics)
                .HasForeignKey(fe => fe.FilterId)
                .HasConstraintName("FK_FilterEpics_Filter");

            builder.HasOne(fe => fe.Epic)
                .WithMany()
                .HasForeignKey(fe => fe.EpicId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FilterEpics_Epic");

            builder.HasOne(e => e.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(e => e.LastUpdatedBy)
                .HasConstraintName("FK_FilterEpics_LastUpdatedBy");
        }
    }
}
