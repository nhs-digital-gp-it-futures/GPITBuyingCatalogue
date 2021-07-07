using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Models;
using NHSD.GPIT.BuyingCatalogue.EntityFramework.Models.GPITBuyingCatalogue;

namespace NHSD.GPIT.BuyingCatalogue.EntityFramework.Catalogue.Configuration
{
    internal sealed class CataloguePriceTierEntityTypeConfiguration : IEntityTypeConfiguration<CataloguePriceTier>
    {
        public void Configure(EntityTypeBuilder<CataloguePriceTier> builder)
        {
            builder.ToTable("CataloguePriceTier");

            builder.Property(t => t.Price).HasColumnType("decimal(18, 3)");

            builder.HasOne<CataloguePrice>()
                .WithMany()
                .HasForeignKey(t => t.CataloguePriceId);
        }
    }
}
