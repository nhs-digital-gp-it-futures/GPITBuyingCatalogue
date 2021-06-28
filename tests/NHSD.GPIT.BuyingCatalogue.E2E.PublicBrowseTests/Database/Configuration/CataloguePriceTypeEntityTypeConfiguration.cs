using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NHSD.GPIT.BuyingCatalogue.E2ETests.Database.Models;

namespace NHSD.GPIT.BuyingCatalogue.E2ETests.Database.Configuration
{
    internal sealed class CataloguePriceTypeEntityTypeConfiguration : IEntityTypeConfiguration<CataloguePriceType>
    {
        public void Configure(EntityTypeBuilder<CataloguePriceType> builder)
        {
            builder.ToTable("CataloguePriceType");

            builder.Property(e => e.CataloguePriceTypeId).ValueGeneratedNever();
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(10);
        }
    }
}
