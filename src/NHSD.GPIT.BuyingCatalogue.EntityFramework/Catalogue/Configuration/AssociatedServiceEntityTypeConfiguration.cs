using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class AssociatedServiceEntityTypeConfiguration : IEntityTypeConfiguration<AssociatedService>
    {
        public void Configure(EntityTypeBuilder<AssociatedService> builder)
        {
            builder.ToTable("AssociatedServices", Schemas.Catalogue);

            builder.HasKey(a => a.CatalogueItemId);

            builder.Property(a => a.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(a => a.Description).HasMaxLength(1000);
            builder.Property(a => a.OrderGuidance).HasMaxLength(1000);
            builder.Property(e => e.PracticeReorganisationType).HasConversion<int>();

            builder.Property(a => a.LastUpdated).HasDefaultValue(DateTime.UtcNow);

            builder.HasOne(a => a.CatalogueItem)
                .WithOne(i => i.AssociatedService)
                .HasForeignKey<AssociatedService>(a => a.CatalogueItemId)
                .HasConstraintName("FK_SupplierService_CatalogueItem");

            builder.HasOne(a => a.LastUpdatedByUser)
                .WithMany()
                .HasForeignKey(a => a.LastUpdatedBy)
                .HasConstraintName("FK_AssociatedServices_LastUpdatedBy");
        }
    }
}
