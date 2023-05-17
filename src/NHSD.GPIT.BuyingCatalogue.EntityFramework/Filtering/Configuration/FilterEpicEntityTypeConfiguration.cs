using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Configuration
{
    internal sealed class FilterEpicEntityTypeConfiguration : IEntityTypeConfiguration<FilterEpic>
    {
        public void Configure(EntityTypeBuilder<FilterEpic> builder)
        {
            builder.ToTable("FilterEpics", Schemas.Filtering, b => b.IsTemporal(
                temp =>
                {
                    temp.UseHistoryTable("FilterEpics_History");
                    temp.HasPeriodStart("SysStartTime");
                    temp.HasPeriodEnd("SysEndTime");
                }));

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
