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
            builder.HasKey(sc => new { sc.CatalogueItemId, sc.CapabilityId });

            builder.ToTable("CatalogueItemCapabilities", Schemas.Catalogue);

            builder.Property(sc => sc.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.HasOne(sc => sc.Capability)
                .WithMany()
                .HasForeignKey(sc => sc.CapabilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolutionCapability_Capability");

            builder.HasOne<CatalogueItem>()
                .WithMany(s => s.CatalogueItemCapabilities)
                .HasForeignKey(sc => sc.CatalogueItemId)
                .HasConstraintName("FK_SolutionCapability_Solution");

            builder.HasOne(sc => sc.Status)
                .WithMany()
                .HasForeignKey(sc => sc.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SolutionCapability_SolutionCapabilityStatus");
        }
    }
}
