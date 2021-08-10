using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class SolutionEntityTypeConfiguration : IEntityTypeConfiguration<Solution>
    {
        public void Configure(EntityTypeBuilder<Solution> builder)
        {
            builder.ToTable("Solutions", Schemas.Catalogue);

            builder.HasKey(s => s.CatalogueItemId);

            builder.Property(s => s.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(s => s.AboutUrl).HasMaxLength(1000);
            builder.Property(s => s.FullDescription).HasMaxLength(3000);
            builder.Property(s => s.ImplementationDetail).HasMaxLength(1100);
            builder.Property(s => s.IntegrationsUrl).HasMaxLength(1000);
            builder.Property(s => s.RoadMap).HasMaxLength(1000);
            builder.Property(s => s.ServiceLevelAgreement).HasMaxLength(1000);
            builder.Property(s => s.Summary).HasMaxLength(350);
            builder.Property(s => s.Version).HasMaxLength(10);

            builder.HasOne(s => s.CatalogueItem)
                .WithOne(i => i.Solution)
                .HasForeignKey<Solution>(s => s.CatalogueItemId)
                .HasConstraintName("FK_Solution_CatalogueItem");
        }
    }
}
