﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class CatalogueItemCapabilityEntityTypeConfiguration : IEntityTypeConfiguration<CatalogueItemCapability>
    {
        public void Configure(EntityTypeBuilder<CatalogueItemCapability> builder)
        {
            builder.HasKey(i => new { i.CatalogueItemId, i.CapabilityId });

            builder.ToTable("CatalogueItemCapabilities", Schemas.Catalogue);

            builder.Property(i => i.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(i => i.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(i => i.Capability)
                .WithMany(c => c.CatalogueItemCapabilities)
                .HasForeignKey(c => c.CapabilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CatalogueItemCapabilities_Capability");

            builder.HasOne(cic => cic.CatalogueItem)
                .WithMany(i => i.CatalogueItemCapabilities)
                .HasForeignKey(c => c.CatalogueItemId)
                .HasConstraintName("FK_CatalogueItemCapabilities_CatalogueItem");

            builder.HasOne(i => i.Status)
                .WithMany()
                .HasForeignKey(c => c.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CatalogueItemCapabilities_Status");

            builder.HasOne(i => i.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(i => i.LastUpdatedBy)
                .HasConstraintName("FK_CatalogueItemCapabilities_LastUpdatedBy");
        }
    }
}
