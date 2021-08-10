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

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id).ValueGeneratedNever();
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(10);
        }
    }
}
