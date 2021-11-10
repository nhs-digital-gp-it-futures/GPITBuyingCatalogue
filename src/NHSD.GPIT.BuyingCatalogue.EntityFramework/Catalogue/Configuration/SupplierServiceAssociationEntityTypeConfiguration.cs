using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class SupplierServiceAssociationEntityTypeConfiguration : IEntityTypeConfiguration<SupplierServiceAssociation>
    {
        public void Configure(EntityTypeBuilder<SupplierServiceAssociation> builder)
        {
            builder.ToTable("SupplierServiceAssociations", Schemas.Catalogue);

            builder.HasKey(e => new { e.AssociatedServiceId, e.CatalogueItemId });

            builder.Property(ssa => ssa.AssociatedServiceId)
                .IsRequired()
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(ssa => ssa.CatalogueItemId)
                .IsRequired()
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(ssa => ssa.LastUpdated).HasDefaultValue(DateTime.UtcNow);
            builder.Property(ssa => ssa.LastUpdatedBy);

            builder.HasOne(d => d.CatalogueItem)
                .WithMany(p => p.SupplierServiceAssociations)
                .HasForeignKey(d => d.CatalogueItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupplierServiceAssociations_CatalogueItem");

            builder.HasOne(ssa => ssa.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(ssa => ssa.LastUpdatedBy)
                .HasConstraintName("FK_SupplierServiceAssociations_LastUpdatedBy");
        }
    }
}
