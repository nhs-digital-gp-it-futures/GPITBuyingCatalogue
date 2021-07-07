using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class AssociatedServiceEntityTypeConfiguration : IEntityTypeConfiguration<AssociatedService>
    {
        public void Configure(EntityTypeBuilder<AssociatedService> builder)
        {
            builder.ToTable("AssociatedService");

            builder.Property(a => a.AssociatedServiceId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(a => a.Description).HasMaxLength(1000);
            builder.Property(a => a.OrderGuidance).HasMaxLength(1000);

            builder.HasOne(a => a.CatalogueItem)
                .WithOne(i => i.AssociatedService)
                .HasForeignKey<AssociatedService>(a => a.AssociatedServiceId)
                .HasConstraintName("FK_SupplierService_CatalogueItem");
        }
    }
}
