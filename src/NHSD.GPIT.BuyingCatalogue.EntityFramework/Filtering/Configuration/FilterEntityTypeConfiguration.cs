using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Filtering.Configuration
{
    internal sealed class FilterEntityTypeConfiguration : IEntityTypeConfiguration<Filter>
    {
        public void Configure(EntityTypeBuilder<Filter> builder)
        {
            builder.ToTable("Filters", Schemas.Filtering, b => b.IsTemporal(
                temp =>
                {
                    temp.UseHistoryTable("Filters_History");
                    temp.HasPeriodStart("SysStartTime");
                    temp.HasPeriodEnd("SysEndTime");
                }));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).ValueGeneratedOnAdd();

            builder.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(f => f.Description)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(f => f.OrganisationId)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(f => f.FrameworkId)
                .HasMaxLength(36);

            builder.Property(i => i.Created).HasDefaultValue(DateTime.UtcNow);

            builder.Property(i => i.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(i => i.Organisation)
                .WithMany()
                .HasForeignKey(i => i.OrganisationId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Filters_OrganisationId");

            builder.HasOne(i => i.Framework)
                .WithMany()
                .HasForeignKey(i => i.FrameworkId)
                .OnDelete(DeleteBehavior.NoAction)
                .HasConstraintName("FK_Filters_FrameworkId");

            builder.HasOne(i => i.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(i => i.LastUpdatedBy)
                .HasConstraintName("FK_Filters_LastUpdatedBy");

            builder.HasIndex(o => o.IsDeleted, "IX_Filters_IsDeleted");

            builder.HasIndex(
                o => new
                {
                    o.Id,
                    o.IsDeleted,
                },
                "IX_Id_IsDeleted_Revision");

            builder.HasQueryFilter(o => !o.IsDeleted);

            builder.HasMany(x => x.FilterCapabilityEpics)
                .WithOne()
                .HasForeignKey(x => x.FilterId);
        }
    }
}
