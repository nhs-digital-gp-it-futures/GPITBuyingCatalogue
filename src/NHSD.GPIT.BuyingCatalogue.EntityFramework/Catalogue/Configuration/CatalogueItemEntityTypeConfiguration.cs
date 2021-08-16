using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class CatalogueItemEntityTypeConfiguration : IEntityTypeConfiguration<CatalogueItem>
    {
        public void Configure(EntityTypeBuilder<CatalogueItem> builder)
        {
            builder.ToTable("CatalogueItems", Schemas.Catalogue);

            builder.Property(i => i.Id)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

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

            builder.HasOne(i => i.Supplier)
                .WithMany(s => s.CatalogueItems)
                .HasForeignKey(i => i.SupplierId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CatalogueItems_Supplier");

            builder.HasIndex(i => new { i.SupplierId, i.Name }, "AK_CatalogueItems_Supplier_Name")
                .IsUnique();
        }
    }
}
