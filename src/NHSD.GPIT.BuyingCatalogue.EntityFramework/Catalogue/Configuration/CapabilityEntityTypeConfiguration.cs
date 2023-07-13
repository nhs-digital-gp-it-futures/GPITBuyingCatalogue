using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class CapabilityEntityTypeConfiguration : IEntityTypeConfiguration<Capability>
    {
        public void Configure(EntityTypeBuilder<Capability> builder)
        {
            builder.ToTable("Capabilities", Schemas.Catalogue);

            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.CapabilityRef, "IX_Capabilities_CapabilityRef");

            builder.Property(c => c.Id).ValueGeneratedNever();
            builder.Property(c => c.CapabilityRef)
                .HasMaxLength(10)
                .HasComputedColumnSql("'C'+CONVERT([nvarchar](3),[Id])");

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.EffectiveDate).HasColumnType("date");
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(c => c.SourceUrl).HasMaxLength(1000);
            builder.Property(c => c.Status)
                .HasConversion<int>()
                .HasColumnName("StatusId");

            builder.Property(c => c.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasMany(c => c.CatalogueItemCapabilities)
                .WithOne(cic => cic.Capability)
                .HasForeignKey(c => c.CapabilityId);

            builder.HasMany(c => c.FrameworkCapabilities)
                .WithOne(f => f.Capability);

            builder.Property(c => c.Version)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasOne(c => c.Category)
                .WithMany(cc => cc.Capabilities)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Capabilities_CapabilityCategory");

            builder.HasMany(c => c.StandardCapabilities)
                .WithOne(sc => sc.Capability)
                .HasForeignKey(c => c.CapabilityId);

            builder.HasOne(c => c.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(c => c.LastUpdatedBy);

            builder.HasMany(x => x.Epics)
                .WithMany(x => x.Capabilities)
                .UsingEntity<CapabilityEpic>(
                    right => right.HasOne<Epic>(c => c.Epic)
                        .WithMany(x => x.CapabilityEpics)
                        .HasForeignKey(nameof(CapabilityEpic.EpicId))
                        .HasConstraintName("FK_CapabilityEpics_Epic"),
                    left => left.HasOne<Capability>()
                        .WithMany(x => x.CapabilityEpics)
                        .HasForeignKey(nameof(CapabilityEpic.CapabilityId))
                        .HasConstraintName("FK_CapabilityEpics_Capability"),
                    j =>
                    {
                        j.ToTable("CapabilityEpics", Schemas.Catalogue);
                        j.HasKey("CapabilityId", "EpicId");
                        j.Property(e => e.CompliancyLevel)
                            .HasConversion<int>()
                            .HasColumnName("CompliancyLevelId");
                    });
        }
    }
}
