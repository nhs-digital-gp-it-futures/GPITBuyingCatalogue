using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Ordering.Models;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class AdditionalServiceEntityTypeConfiguration : IEntityTypeConfiguration<AdditionalService>
    {
        public void Configure(EntityTypeBuilder<AdditionalService> builder)
        {
            builder.ToTable("AdditionalService");

            builder.HasKey(a => a.CatalogueItemId);

            builder.Property(a => a.CatalogueItemId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(a => a.FullDescription).HasMaxLength(3000);
            builder.Property(a => a.SolutionId)
                .HasMaxLength(14)
                .HasConversion(id => id.ToString(), id => CatalogueItemId.ParseExact(id));

            builder.Property(a => a.Summary).HasMaxLength(300);

            builder.HasOne(a => a.CatalogueItem)
                .WithOne(i => i.AdditionalService)
                .HasForeignKey<AdditionalService>(d => d.CatalogueItemId)
                .HasConstraintName("FK_AdditionalService_CatalogueItem");

            builder.HasOne(a => a.Solution)
                .WithMany(s => s.AdditionalServices)
                .HasForeignKey(a => a.SolutionId)
                .HasConstraintName("FK_AdditionalService_Solution");
        }
    }
}
