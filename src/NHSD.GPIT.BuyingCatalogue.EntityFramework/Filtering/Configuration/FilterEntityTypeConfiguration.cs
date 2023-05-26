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

            builder.HasMany(x => x.Capabilities)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    r => r.HasOne<Capability>()
                        .WithMany()
                        .HasForeignKey("CapabilityId")
                        .HasConstraintName("FK_FrameworkCapabilities_Capability"),
                    l => l.HasOne<Filter>()
                        .WithMany()
                        .HasForeignKey("FilterId")
                        .HasConstraintName("FK_FrameworkCapabilities_Filter"),
                    j =>
                    {
                        j.ToTable(
                            "FilterCapabilities",
                            b => b.IsTemporal(
                                temp =>
                                {
                                    temp.UseHistoryTable("AspNetUsers_History");
                                    temp.HasPeriodStart("SysStartTime");
                                    temp.HasPeriodEnd("SysEndTime");
                                }));
                        j.HasKey("CapabilityId", "FilterId");
                    });

            builder.HasMany(x => x.Epics)
                .WithMany()
                .UsingEntity<Dictionary<string, object>>(
                    r => r.HasOne<Epic>()
                        .WithMany()
                        .HasForeignKey("EpicId")
                        .HasConstraintName("FK_FrameworkEpics_Epic"),
                    l => l.HasOne<Filter>()
                        .WithMany()
                        .HasForeignKey("FilterId")
                        .HasConstraintName("FK_FrameworkEpics_Filter"),
                    j =>
                    {
                        j.ToTable(
                            "FilterEpics",
                            b => b.IsTemporal(
                                temp =>
                                {
                                    temp.UseHistoryTable("AspNetUsers_History");
                                    temp.HasPeriodStart("SysStartTime");
                                    temp.HasPeriodEnd("SysEndTime");
                                }));
                        j.HasKey("EpicId", "FilterId");
                    });
        }
    }
}
