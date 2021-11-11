using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.ValueGenerators;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class CatalogueItemEntityTypeConfiguration : IEntityTypeConfiguration<CatalogueItem>
    {
        public void Configure(EntityTypeBuilder<CatalogueItem> builder)
        {
            builder.ToTable("CatalogueItems", Schemas.Catalogue);

            builder.Property(i => i.Id)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id))
                .HasValueGenerator<CatalogueItemIdValueGenerator>();

            builder.Property(i => i.CatalogueItemType)
                .HasConversion<int>()
                .HasColumnName("CatalogueItemTypeId");

            builder.Property(i => i.Created).HasDefaultValue(DateTime.UtcNow);
            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(i => i.PublishedStatus)
                .HasConversion<int>()
                .HasColumnName("PublishedStatusId")
                .HasDefaultValue(PublicationStatus.Draft);

            builder.Property(i => i.SupplierId)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(i => i.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(i => i.Supplier)
                .WithMany(s => s.CatalogueItems)
                .HasForeignKey(i => i.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CatalogueItems_Supplier");

            builder.HasMany(i => i.CatalogueItemContacts)
                .WithMany(s => s.AssignedCatalogueItems)
                .UsingEntity<Dictionary<string, object>>(
                    "CatalogueItemContacts",
                    right => right.HasOne<SupplierContact>().WithMany().HasForeignKey("SupplierContactId"),
                    left => left.HasOne<CatalogueItem>().WithMany().HasForeignKey("CatalogueItemId"));

            builder.HasIndex(i => new { i.SupplierId, i.Name }, "AK_CatalogueItems_Supplier_Name")
                .IsUnique();

            builder.HasOne(i => i.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(i => i.LastUpdatedBy)
                .HasConstraintName("FK_CatalogueItems_LastUpdatedBy");
        }
    }
}
