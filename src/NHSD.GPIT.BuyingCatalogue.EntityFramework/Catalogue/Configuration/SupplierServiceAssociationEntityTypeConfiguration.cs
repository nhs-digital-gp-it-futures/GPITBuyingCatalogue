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
            builder.ToTable("SupplierServiceAssociation");

            builder.HasNoKey();

            builder.Property(ssa => ssa.AssociatedServiceId)
                .IsRequired()
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(ssa => ssa.CatalogueItemId)
                .IsRequired()
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.HasOne(ssa => ssa.AssociatedService)
                .WithMany()
                .HasForeignKey(ssa => ssa.AssociatedServiceId)
                .HasConstraintName("FK_SupplierServiceAssociation_AssociatedService");

            builder.HasOne(ssa => ssa.CatalogueItem)
                .WithMany()
                .HasForeignKey(ssa => ssa.CatalogueItemId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SupplierServiceAssociation_CatalogueItem");
        }
    }
}
